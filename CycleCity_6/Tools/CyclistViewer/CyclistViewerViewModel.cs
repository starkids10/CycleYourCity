using System;
using CycleCity_6.Materials;
using CycleCity_6.Services;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Timers;
using System.Windows.Media;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : ToolViewModel
    {
        private Timer aTimer;

        public CyclistViewerViewModel(TrackService trackService)
        {
            Contract.Requires (trackService != null);

            Tracks = new ObservableCollection<Track> (trackService.GetAllTracks ());

            trackService.TrackAddedEvent += TrackService_OnTrackAdded;

            aTimer = new Timer (1000);
            aTimer.Elapsed += CollectData_OnTimedEvent;
            aTimer.Enabled = true;
        }

        public ObservableCollection<Track> Tracks { get; }

        private GraphicsLayer _mapLayer;
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

                AddCyclistsToMapLayer (value);
            }
        }

        public bool HasMapLayer()
        {
            return _mapLayer != null;
        }

        private void AddCyclistsToMapLayer(GraphicsLayer mapLayer)
        {
            Contract.Requires (mapLayer != null);

            foreach(var cyclist in Tracks)
            {
                AddCyclistToMapLayer (mapLayer, cyclist);
            }
        }

        /// <summary>
        /// adds new track to map and generates the color of the track
        /// </summary>
        /// <param name="mapLayer"></param>
        /// <param name="track"></param>
        private static void AddCyclistToMapLayer(GraphicsLayer mapLayer, Track track)
        {
            Contract.Requires (mapLayer != null);
            Contract.Requires (track != null);
            var simpleLineSymbol = new SimpleLineSymbol ();
            simpleLineSymbol.Width = 3;
            Random randomGen = new Random ();
            var randomColor = Color.FromRgb ((byte)randomGen.Next (255), (byte)randomGen.Next (255),
                (byte)randomGen.Next (255));
            simpleLineSymbol.Color = randomColor;
            mapLayer.Graphics.Add (new Graphic (track.Tour, simpleLineSymbol));
        }

        private void TrackService_OnTrackAdded(object sender, Track newCyclist)
        {
            Contract.Requires (newCyclist != null);

            Tracks.Add (newCyclist);

            if(HasMapLayer ())
            {
                AddCyclistToMapLayer (MapLayer, newCyclist);
            }
        }

        //TODO erst einkommentieren wenn wir daten vom Server kriegen
        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            /*
            string data = DatabaseContentService.GetNewData();

            Polyline newTrack = GpsToEsriParser.ParseJsonToEsriPolyline(data);

            // TODO Auflösung der Cyclist --> umstellung auf Track??
            Cyclist newCyclist = new Cyclist(0, "", newTrack);
            if (HasMapLayer())
            {
                AddCyclistToMapLayer(MapLayer, newCyclist);
            }
            */
        }
    }
}
