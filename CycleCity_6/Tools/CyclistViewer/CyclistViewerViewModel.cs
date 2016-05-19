using System;
using System.Collections.Generic;
using CycleCity_6.Materials;
using CycleCity_6.Services;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

using System.Windows.Media;

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : ToolViewModel
    {

        public ObservableCollection<Track> Tracks { get; }
        public ObservableCollection<HeatPoint> HeatMap { get; } 

        private GraphicsLayer _mapLayer;


        public CyclistViewerViewModel(TrackService trackService)
        {
            Contract.Requires (trackService != null);

            Tracks = new ObservableCollection<Track> (trackService.GetAllTracks ());
            HeatMap = new ObservableCollection<HeatPoint>(trackService.GetAllHeatPoints());
            trackService.TrackAddedEvent += TrackService_OnTrackAdded;
            trackService.HeatPointAddedEvent += TrackService_OnHeatMapChanged;
        }

        public GraphicsLayer MapLayer
        {
            get
            {
                Contract.Requires (HasMapLayer ());

                return _mapLayer;
            }
            set
            {
                Contract.Requires (value != null);
                Contract.Ensures (HasMapLayer ());

                _mapLayer = value;

                AddTracksToMapLayer (value);
            }
        }

        public bool HasMapLayer()
        {
            return _mapLayer != null;
        }

        private void AddTracksToMapLayer(GraphicsLayer mapLayer)
        {
            Contract.Requires (mapLayer != null);

            foreach(var track in Tracks)
            {
                AddTrackToMapLayer (mapLayer, track);
            }
        }

        private void AddHeatmapToMapLayer(GraphicsLayer mapLayer)
        {
            Contract.Requires(mapLayer != null);
            foreach (HeatPoint heatPoint in HeatMap)
            {
                AddHeatpointToMapLayer(mapLayer,heatPoint);
            }

        }

        /// <summary>
        /// adds new track to map and generates the color of the track
        /// </summary>
        /// <param name="mapLayer"></param>
        /// <param name="track"></param>
        private void AddTrackToMapLayer(GraphicsLayer mapLayer, Track track)
        {
            Contract.Requires (mapLayer != null);
            Contract.Requires (track != null);
            var simpleLineSymbol = new SimpleLineSymbol {Width = 3};
            Random randomGen = new Random ();
            var randomColor = Color.FromRgb ((byte)randomGen.Next (255), (byte)randomGen.Next (255),
                (byte)randomGen.Next (255));
            simpleLineSymbol.Color = randomColor;
            mapLayer.Graphics.Add (new Graphic (track.Tour, simpleLineSymbol));
        }

        private void AddHeatpointToMapLayer(GraphicsLayer mapLayer, HeatPoint heatPoint)
        {
            var punktStyle = new SimpleMarkerSymbol();
            var heat = heatPoint.Heat;
            if (heat < 5)
            {
                punktStyle.Color = Colors.Aqua;
                punktStyle.Size = 1;
            }
            else if (heat < 10)
            {
                punktStyle.Color = Colors.Blue;
                punktStyle.Size = 3;
            }
            else if (heat < 20)
            {
                punktStyle.Color = Colors.Tomato;
                punktStyle.Size = 5;
            }
            else
            {
                punktStyle.Color = Colors.Red;
                punktStyle.Size = 7;
            }
            List<Point> points = heatPoint.Points;
            foreach (Point point in points)
            {
                mapLayer.Graphics.Add(new Graphic(point.Coordinates,punktStyle));
            }
        }

        private void TrackService_OnHeatMapChanged(object sender, IEnumerable<HeatPoint> heatPoints)
        {
            HeatMap.Concat(heatPoints);

            if (HasMapLayer())
            {
                AddHeatmapToMapLayer(MapLayer);
            }
        }

        private void TrackService_OnTrackAdded(object sender, Track track)
        {
            Contract.Requires (track != null);

            Tracks.Add (track);

            if(HasMapLayer ())
            {
                AddTrackToMapLayer (MapLayer, track);
            }
        }
    }
}
