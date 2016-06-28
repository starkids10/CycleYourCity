using System;
using System.Collections;
using System.Collections.Generic;
using CycleCity_6.Materials;
using CycleCity_6.Services;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Geometry;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

using System.Windows.Media;
using System.ComponentModel;
using System.Net;
using System.Windows;
using Point = CycleCity_6.Materials.Point;

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : INotifyPropertyChanged
    {
        private string _LetzteAktuallisierung;

        private GraphicsLayer trackGraphicsLayer;
        private GraphicsLayer veloGraphicsLayer;
        private List<Graphic> veloGraphics; 
        public MapView mapView;
        private readonly TrackService _trackService;
        private readonly LocalDBService _localDBService;

        public CyclistViewerViewModel(TrackService trackService, LocalDBService localDBService)
        {
            Contract.Requires(trackService != null);

            InitializeMap();

            LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString();
            _localDBService = localDBService;
            _trackService = trackService;
            trackService.TrackAddedEvent += TrackService_OnTrackAdded;
            trackService.HeatPointAddedEvent += TrackService_OnHeatMapChanged;
            trackService.KeineInternetVerbindungEvent += TrackService_OnKeineInternetVerbindung;

            InitializeVelorouten();

            }

        private void InitializeVelorouten()
        {
            veloGraphics = new List<Graphic>();
            foreach (var velo in _trackService.Velorouten)
            {
                var simpleLineSymbol = new SimpleLineSymbol
                {
                    Color = Color.FromArgb(80,255,0,0),
                    Width = 4
                };
                veloGraphics.Add(new Graphic(velo.Tour, simpleLineSymbol));
            }
        }

        public Map Map
        {
            get;
            private set;
        }

        public string LetzteAktuallisierung
        {
            get { return _LetzteAktuallisierung; }
            private set { _LetzteAktuallisierung = value; Notify("LetzteAktuallisierung"); }
        }

        public void InitializeMap()
        {
            Map = new Map();

            // create a new layer (world street map tiled layer)
            var uriStreet = new Uri ("http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer");
            var uriDark = new Uri("http://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Dark_Gray_Base/MapServer");
            var uriLight = new Uri ("http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer");
            var baseLayer = new Esri.ArcGISRuntime.Layers.ArcGISTiledMapServiceLayer(uriLight);
            // (give the layer an ID so it can be found later)
            baseLayer.ID = "BaseMap";

            trackGraphicsLayer = new GraphicsLayer();
            veloGraphicsLayer = new GraphicsLayer();

            // add the layer to the Map
            Map.Layers.Add(baseLayer);
            Map.Layers.Add(veloGraphicsLayer);
            Map.Layers.Add(trackGraphicsLayer);
            // set the initial view point
            var mapPoint = new Esri.ArcGISRuntime.Geometry.MapPoint(9.993888, 53.548401,
                Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
            var initViewPoint = new Esri.ArcGISRuntime.Controls.ViewpointCenter(mapPoint, 250000);

            Map.InitialViewpoint = initViewPoint;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(string argument)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(argument));
        }

        private void AddHeatmapToMapLayer(List<Graphic> collection, IEnumerable<HeatPoint> HeatMap)
        {
            foreach (HeatPoint heatPoint in HeatMap)
            {
                AddHeatpointToMapLayer(collection, heatPoint);
            }
        }

        private void AddTrackToMapLayer(List<Graphic> collection, Track track)
        {
            Contract.Requires(track != null);

            var simpleLineSymbol = new SimpleLineSymbol { Width = 3 };
            Random randomGen = new Random();
            var randomColor = Color.FromRgb((byte)randomGen.Next(255), (byte)randomGen.Next(255), (byte)randomGen.Next(255));

            simpleLineSymbol.Color = randomColor;
            collection.Add(new Graphic(track.Tour, simpleLineSymbol));

            mapView.Dispatcher.InvokeAsync (() => trackGraphicsLayer.Graphics.Clear ());
            mapView.Dispatcher.InvokeAsync (() => trackGraphicsLayer.Graphics.AddRange(collection));

            LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString ();
        }

        private void AddHeatpointToMapLayer(List<Graphic> collection, HeatPoint heatPoint)
        {
            _trackService.AktiviereUpdate(false);
            var punktStyle = new SimpleMarkerSymbol();
            var heat = heatPoint.Heat;

            if (heat < 5)
            {
                punktStyle.Color = Colors.Aqua;
                punktStyle.Size = 2;
            }
            else if (heat < 10)
            {
                punktStyle.Color = Colors.Blue;
                punktStyle.Size = 4;
            }
            else if (heat < 20)
            {
                punktStyle.Color = Colors.Tomato;
                punktStyle.Size = 6;
            }
            else
            {
                punktStyle.Color = Colors.Red;
                punktStyle.Size = 8;
            }

            List<Point> points = heatPoint.Points;
            Point[] pointsClone = new Point[points.Count];
            // TODO hier fliegt er raus wenn die heatmap zu lange offen istpoints.ToArray().CopyTo(pointsClone, 0);

            foreach (Point point in pointsClone)
            {
                collection.Add(new Graphic(point.Coordinates, punktStyle));

            }

            mapView.Dispatcher.InvokeAsync(() => trackGraphicsLayer.Graphics.Clear());
            mapView.Dispatcher.InvokeAsync(() => trackGraphicsLayer.Graphics.AddRange(collection));
            _trackService.AktiviereUpdate(true);
        }

        private void TrackService_OnHeatMapChanged(object sender, IEnumerable<HeatPoint> heatPoints)
        {
            AddHeatmapToMapLayer(new List<Graphic>(), heatPoints);
        }

        private void TrackService_OnTrackAdded(object sender, List<Track> tracks)
        {
            Contract.Requires(tracks != null);
            AddTracksToMapLayer(tracks);
        }

        public void AddTracksToMapLayer(List<Track> tracks)
        {
            List<Graphic> collection = new List<Graphic> ();

            foreach(var track in tracks)
            {
                var simpleLineSymbol = new SimpleLineSymbol { Width = 3, Color = Color.FromArgb(40,0,0,255)};
                
                collection.Add (new Graphic (track.Tour, simpleLineSymbol));
                Console.WriteLine (track.Id);
                Console.WriteLine ("---");
            }

            mapView.Dispatcher.InvokeAsync (() => trackGraphicsLayer.Graphics.Clear ());
            mapView.Dispatcher.InvokeAsync(() => veloGraphicsLayer.Graphics.AddRange(veloGraphics));
            mapView.Dispatcher.InvokeAsync (() => trackGraphicsLayer.Graphics.AddRange (collection));

            LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString ();
        }


        private void TrackService_OnKeineInternetVerbindung(object sender, UnhandledExceptionEventArgs e)
        {
            var result = MessageBox.Show("Es besteht keine Verbindung zum Server. Drücken Sie 'Ok' um es erneut zu versuchen oder 'Cancel' zum Beenden des Programms.", "Netzwerkfehler", MessageBoxButton.OKCancel,
                   MessageBoxImage.Error);
            if (result == MessageBoxResult.OK)
            {
                _trackService.AktiviereUpdate(true);
            }
            else
            {
                Environment.Exit(-1);
            }
        }

        public void HeatmapAnzeigen(bool heatmapanzeigen)
        {
            _trackService.HeatmapAnzeigen = heatmapanzeigen;
        }


        
        /// <summary>
        /// Setzt das Datum und die Uhrzeit für Start und Endpunkt des anzuzeigenden Zeitintervalls.
        /// Wenn der Endzeitpunkt DateTime.Min_Value ist, werden alle Daten vom Startzeitpunkt aus angefordert.
        /// </summary>
        /// <param name="startzeit">Start des Zeitintervalls.</param>
        /// <param name="endzeit">Ende des Zeitintervalls</param>
        public void SetzeUhrzeit(DateTime startzeit, DateTime endzeit)
        {
            _trackService.Startzeit = startzeit;
            _trackService.Endzeit = endzeit;
        }


    }
}
