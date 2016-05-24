using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Timer = System.Timers.Timer;

namespace CycleCity_6.Services
{
    internal class TrackService
    {
        private readonly List<Track> _tracks;
        private readonly Dictionary<string, HeatPoint> _heatPoints;
        private Timer aTimer;
        private readonly DatabaseContentService _databaseContentService;

        public TrackService()
        {
            _databaseContentService = new DatabaseContentService();
            _tracks = new List<Track>();
            _heatPoints = new Dictionary<string, HeatPoint>();

            aTimer = new Timer (1000);
            aTimer.Elapsed += CollectData_OnTimedEvent;
            aTimer.Enabled = true;
        }

        public event EventHandler<Track> TrackAddedEvent = delegate { };
        public event EventHandler<IEnumerable<HeatPoint>> HeatPointAddedEvent = delegate { };

        /// <summary>
        /// Returns all tracks.
        /// </summary>
        /// <returns>all registered tracks</returns>
        public IEnumerable<Track> GetAllTracks()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Track>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<Track>>().Any());
            return _tracks;
        }


        public IEnumerable<HeatPoint> GetAllHeatPoints()
        {
            Contract.Ensures(Contract.Result<IEnumerable<HeatPoint>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<HeatPoint>>().Any());
            return _heatPoints.Values;
        } 

        /// <summary>
        /// Adds a new Tour to the list
        /// </summary>
        /// <param name="id">id of the track</param>
        /// <param tour="tour">Polyline for the track which simulates the tour</param>
        public void AddNewTrack(string id, Polyline tour)
        {
            var newTrack = new Track(id, tour);

            _tracks.Add(newTrack);

            TrackAddedEvent(this, newTrack);
        }

        /// <summary>
        /// Generiert die HeatMap neu.
        /// </summary>
        /// <param name="newPoints">Liste von neu hinzuzufügenden Punkten</param>
        public async void GenerateNewHeatMap(IEnumerable<Point> newPoints)
        {
            var locator = new Esri.ArcGISRuntime.Tasks.Geocoding.OnlineLocatorTask(new Uri(@"http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer"), String.Empty);
            foreach (Point newPoint in newPoints)
            {
                var addressInfo =
                    await
                        locator.ReverseGeocodeAsync(newPoint.Coordinates,50, newPoint.Coordinates.SpatialReference,
                            CancellationToken.None);
                string adresse = addressInfo.AddressFields["Address"];
                HeatPoint heatPoint = null;
                if (_heatPoints.TryGetValue(adresse, out heatPoint))
                {
                    heatPoint.Points.Add(newPoint);
                }
                else
                {
                    _heatPoints.Add(adresse, new HeatPoint(new List<Point>() {newPoint}));
                }

            }
        }

        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            var data = _databaseContentService.GetNewData();
            var tracks = GpsToEsriParser.ParseJsonToEsriPolyline(data);
            foreach (Track track in tracks)
            {
                TrackAddedEvent(this, track);
            }

            //var heatPoints = GpsToEsriParser.ParseJsonToPoinList(data);
            //GenerateNewHeatMap(heatPoints);
            //HeatPointAddedEvent(this, GetAllHeatPoints());
        }
    }
}
