﻿using System;
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

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : ToolViewModel
    {

        public event EventHandler<List<Graphic>> GraphicsCollection = delegate { };

        public ObservableCollection<Track> Tracks { get; }
        //public ObservableCollection<HeatPoint> HeatMap { get; } 

        public CyclistViewerViewModel(TrackService trackService)
        {
            Contract.Requires (trackService != null);

            //InitializeMap ();

            //Tracks = new ObservableCollection<Track> (trackService.GetAllTracks ());
            //HeatMap = new ObservableCollection<HeatPoint>(trackService.GetAllHeatPoints());
            trackService.TrackAddedEvent += TrackService_OnTrackAdded;
            trackService.HeatPointAddedEvent += TrackService_OnHeatMapChanged;

            List<Track> trackList = GpsToEsriParser.ParseJsonToEsriPolyline (trackService.data);

        }

        public GraphicsLayer MapLayer
        {
            get;
            set;
        }

        private void AddTracksToMapLayer(List<Graphic> collection)
        {
            Contract.Requires (MapLayer != null);

            foreach(var track in Tracks)
            {
                AddTrackToMapLayer (collection, track);
            }
        }

        private void AddHeatmapToMapLayer(List<Graphic> collection, IEnumerable<HeatPoint> HeatMap)
        {
            Contract.Requires(MapLayer != null);
            foreach (HeatPoint heatPoint in HeatMap)
            {
                AddHeatpointToMapLayer(collection,heatPoint);
            }

        }

        /// <summary>
        /// adds new track to map and generates the color of the track
        /// </summary>
        /// <param name="mapLayer"></param>
        /// <param name="track"></param>
        private void AddTrackToMapLayer(List<Graphic> collection, Track track)
        {
            Contract.Requires (MapLayer != null);
            Contract.Requires (track != null);
            var simpleLineSymbol = new SimpleLineSymbol {Width = 3};
            Random randomGen = new Random ();
            var randomColor = Color.FromRgb ((byte)randomGen.Next (255), (byte)randomGen.Next (255),
                (byte)randomGen.Next (255));
            simpleLineSymbol.Color = randomColor;
            collection.Add (new Graphic (track.Tour, simpleLineSymbol));
            GraphicsCollection (this, collection);
        }

        private void AddHeatpointToMapLayer(List<Graphic> collection, HeatPoint heatPoint)
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
               collection.Add(new Graphic(point.Coordinates,punktStyle));
            }
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke ((Action)(() =>
            {
                GraphicsCollection (this, collection);
            }));
            GraphicsCollection (this, collection);
        }

        private void TrackService_OnHeatMapChanged(object sender, IEnumerable<HeatPoint> heatPoints)
        {
            //HeatMap.Concat(heatPoints);

            //if (HasMapLayer())
            //{
            //    AddHeatmapToMapLayer(MapLayer);
            //}

            AddHeatmapToMapLayer (new List<Graphic>(), heatPoints);
        }

        private void TrackService_OnTrackAdded(object sender, Track track)
        {
            Contract.Requires (track != null);

            //Tracks.Add (track);

            //if(HasMapLayer ())
            //{
            //    AddTrackToMapLayer (MapLayer, track);
            //}
            AddTrackToMapLayer (new List<Graphic>(), track);
        }
    }
}
