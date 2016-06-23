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

        private GraphicsLayer _trackGraphicsLayer;
        private GraphicsLayer _veloGraphicsLayer;
        private List<List<Graphic>> _veloGraphics;

        public MapView MapView;
        private readonly TrackService _trackService;

        public CyclistViewerViewModel(TrackService trackService)
        {
            Contract.Requires(trackService != null);

            InitializeMap();

            LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString();
            _trackService = trackService;
            trackService.TrackAddedEvent += TrackService_OnTrackAdded;
            trackService.KeineInternetVerbindungEvent += TrackService_OnKeineInternetVerbindung;

            _veloGraphics = new List<List<Graphic>>();
            InitializeVelorouten();
            TempVeloGraphics = new Dictionary<string, List<Graphic>>();
        }

        private void InitializeVelorouten()
        {
            var simpleLineSymbol = new SimpleLineSymbol
            {
                Color = Color.FromArgb(80, 255, 0, 0),
                Width = 4
            };
            foreach (var velo in _trackService.Velorouten)
            {
                List<Polyline> polylines = new List<Polyline>();
                foreach (Track track in velo)
                {
                    polylines.Add(track.Tour);
                }
                List<Graphic> tempGraphics = new List<Graphic>();
                foreach (Polyline polyline in polylines)
                {
                    tempGraphics.Add(new Graphic(polyline, simpleLineSymbol));
                }
                _veloGraphics.Add(tempGraphics);

            }
        }

        public Dictionary<string, List<Graphic>> TempVeloGraphics { get; set; }

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

        public List<Graphic> GetVeloRouteAt(int index)
        {
            return _veloGraphics[index];
        }

        public void InitializeMap()
        {
            Map = new Map();

            // create a new layer (world street map tiled layer)
            var uriStreet = new Uri("http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer");
            var uriDark = new Uri("http://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Dark_Gray_Base/MapServer");
            var uriLight = new Uri("http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer");
            var baseLayer = new Esri.ArcGISRuntime.Layers.ArcGISTiledMapServiceLayer(uriLight);
            // (give the layer an ID so it can be found later)
            baseLayer.ID = "BaseMap";

            _trackGraphicsLayer = new GraphicsLayer();
            _veloGraphicsLayer = new GraphicsLayer();

            // add the layer to the Map
            Map.Layers.Add(baseLayer);
            Map.Layers.Add(_veloGraphicsLayer);
            Map.Layers.Add(_trackGraphicsLayer);
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


        private void TrackService_OnTrackAdded(object sender, List<Track> tracks)
        {
            Contract.Requires(tracks != null);
            AddTracksToMapLayer(tracks);
        }

        public void AddTracksToMapLayer(List<Track> tracks)
        {
            List<Graphic> collection = new List<Graphic>();

            foreach (var track in tracks)
            {
                var simpleLineSymbol = new SimpleLineSymbol { Width = 3, Color = Color.FromArgb(80, 0, 0, 255) };

                collection.Add(new Graphic(track.Tour, simpleLineSymbol));
            }

            MapView.Dispatcher.InvokeAsync(() => _trackGraphicsLayer.Graphics.Clear());
            MapView.Dispatcher.InvokeAsync(() => _trackGraphicsLayer.Graphics.AddRange(collection));

            LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString();
        }

        public void ZeichneVelorouten()
        {
            MapView.Dispatcher.InvokeAsync((() => _veloGraphicsLayer.Graphics.Clear()));
            foreach (var velo in TempVeloGraphics.Values)
            {
                MapView.Dispatcher.InvokeAsync(() => _veloGraphicsLayer.Graphics.AddRange(velo));
            }
        }

        private void TrackService_OnKeineInternetVerbindung(object sender, UnhandledExceptionEventArgs e)
        {
            //var result = MessageBox.Show("Es besteht keine Verbindung zum Server. Drücken Sie 'Ok' um es erneut zu versuchen oder 'Cancel' zum Beenden des Programms.", "Netzwerkfehler", MessageBoxButton.OKCancel,
            //       MessageBoxImage.Error);
            //if (result == MessageBoxResult.OK)
            //{
            //    _trackService.AktiviereLiveUpdate(true);
            //}
            //else
            //{
            //    Environment.Exit(-1);
            //}
        }

        /// <summary>
        /// Setzt das Datum und die Uhrzeit für Start und Endpunkt des anzuzeigenden Zeitintervalls.
        /// Wenn der Endzeitpunkt DateTime.Min_Value ist, werden alle Daten vom Startzeitpunkt aus angefordert.
        /// </summary>
        /// <param name="startzeit">Start des Zeitintervalls.</param>
        /// <param name="endzeit">Ende des Zeitintervalls</param>
        public void SetzeUhrzeit(DateTime startzeit, DateTime endzeit)
        {
            _trackService.UpdateVonBis(startzeit, endzeit);
        }


    }
}
