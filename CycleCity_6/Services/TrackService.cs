﻿using CycleCity_6.Materials;
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

        public String data = @"{'tracks':[
  { 'track_id':'1',
    'WayPoints': [
    {
      'cmt': '0',
      'time': '2011-12-31 23:59:56',
      'lat': '53.55299',
      'lon': '9.93784',
      'ele': '0.0'
    },
    {
      'cmt': '1',
      'time': '2011-12-31 23:59:59',
      'lat': '53.55179',
      'lon': '9.94323',
      'ele': '0.0'
    }
  ]},
  { 'track_id':'2',
    'WayPoints': [
    {
      'cmt': '0',
      'time': '2011-12-31 23:59:56',
      'lat': '53.54617',
      'lon': '9.95066',
      'ele': '0.0'
    },
    {
      'cmt': '1',
      'time': '2011-12-31 23:59:59',
      'lat': '53.54525',
      'lon': '10.00127',
      'ele': '0.0'
    }
  ]}
]}";

        public TrackService()
        {
            _tracks = new List<Track>();
            _heatPoints = new Dictionary<string, HeatPoint>();

            aTimer = new Timer (10000);
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
        public void AddNewTrack(int id, Polyline tour)
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
                Console.WriteLine (adresse);
                HeatPoint heatPoint = null;
                if (_heatPoints.TryGetValue(adresse, out heatPoint))
                {
                    heatPoint.Points.Add(newPoint);
                    Console.WriteLine ("hinzufügen");
                }
                else
                {
                    _heatPoints.Add(adresse, new HeatPoint(new List<Point>() {newPoint}));
                    Console.WriteLine ("neu");
                }

            }
        }

//        //TODO erst einkommentieren wenn wir daten vom Server kriegen
          private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
         {

//            string data = DatabaseContentService.GetNewData();
              

              var heatPoints = GpsToEsriParser.ParseJsonToPoinList(data);
              GenerateNewHeatMap(heatPoints);
              HeatPointAddedEvent(this, GetAllHeatPoints());

         }
    }
}
