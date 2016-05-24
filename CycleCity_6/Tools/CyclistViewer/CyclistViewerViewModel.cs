using System;
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

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : ToolViewModel
    {
        //private String _LetzteAktuallisierung;

        private GraphicsLayer gLayer;
        public MapView mapView;

        public event EventHandler<List<Graphic>> GraphicsCollection = delegate { };

        public CyclistViewerViewModel(TrackService trackService)
        {
            Contract.Requires (trackService != null);

            InitializeMap ();

            //LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString ();

            trackService.TrackAddedEvent += TrackService_OnTrackAdded;
            trackService.HeatPointAddedEvent += TrackService_OnHeatMapChanged;
        }

        public Map Map
        {
            get;
            private set;
        }

        //public String LetzteAktuallisierung
        //{
        //    get { return _LetzteAktuallisierung; }
        //    private set { _LetzteAktuallisierung = value; }
        //}

        public void InitializeMap()
        {
            Map = new Map ();

            // create a new layer (world street map tiled layer)
            var uri = new Uri ("http://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Dark_Gray_Base/MapServer");
            var baseLayer = new Esri.ArcGISRuntime.Layers.ArcGISTiledMapServiceLayer (uri);
            // (give the layer an ID so it can be found later)
            baseLayer.ID = "BaseMap";

            gLayer = new GraphicsLayer ();

            // add the layer to the Map
            Map.Layers.Add (baseLayer);
            Map.Layers.Add(gLayer);
            // set the initial view point
            var mapPoint = new Esri.ArcGISRuntime.Geometry.MapPoint (9.993888, 53.548401,
                Esri.ArcGISRuntime.Geometry.SpatialReferences.Wgs84);
            var initViewPoint = new Esri.ArcGISRuntime.Controls.ViewpointCenter (mapPoint, 250000);

            Map.InitialViewpoint = initViewPoint;            
        }

        private void AddHeatmapToMapLayer(List<Graphic> collection, IEnumerable<HeatPoint> HeatMap)
        {
            //Contract.Requires(MapLayer != null);
            foreach (HeatPoint heatPoint in HeatMap)
            {
                AddHeatpointToMapLayer(collection, heatPoint);
            }
        }

        private void AddTrackToMapLayer(List<Graphic> collection, Track track)
        {
            //Contract.Requires (MapLayer != null);
            Contract.Requires (track != null);

            var simpleLineSymbol = new SimpleLineSymbol {Width = 3};
            Random randomGen = new Random ();
            var randomColor = Color.FromRgb ((byte)randomGen.Next (255), (byte)randomGen.Next (255), (byte)randomGen.Next (255));

            simpleLineSymbol.Color = randomColor;
            collection.Add (new Graphic (track.Tour, simpleLineSymbol));

            //GraphicsCollection (this, collection);
            Console.WriteLine ("Draw Tracks");

            mapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.Clear ());
            mapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.AddRange(collection));
            //TODO aktuelle Zeit aktualisieren
            //LetzteAktuallisierung = "Letzte Aktuallisierung: " + DateTime.Now.ToLongTimeString ();
            //Console.WriteLine (LetzteAktuallisierung);

        }

        private void AddHeatpointToMapLayer(List<Graphic> collection, HeatPoint heatPoint)
        {
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

            foreach (Point point in points)
            {
               collection.Add(new Graphic(point.Coordinates,punktStyle));
            }

            mapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.Clear ());
            mapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.AddRange (collection));
        }

        private void TrackService_OnHeatMapChanged(object sender, IEnumerable<HeatPoint> heatPoints)
        {
            //Liste wird kopiert um gleichzeitigen Zugriff zu verhindern
            var heatPointsCopy = heatPoints.ToList();
            AddHeatmapToMapLayer (new List<Graphic>(), heatPointsCopy);
        }

        private void TrackService_OnTrackAdded(object sender, Track track)
        {
            Contract.Requires (track != null);
            AddTrackToMapLayer (new List<Graphic>(), track);
        }
    }
}
