using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
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
            _databaseContentService = initServerConnection();
            _tracks = new List<Track>();
            _heatPoints = new Dictionary<string, HeatPoint>();

            aTimer = new Timer(1000);
            aTimer.Elapsed += CollectData_OnTimedEvent;
            aTimer.Enabled = true;

        }

        public event EventHandler<List<Track>> TrackAddedEvent = delegate { };
        public event EventHandler<IEnumerable<HeatPoint>> HeatPointAddedEvent = delegate { };
        public event UnhandledExceptionEventHandler KeineInternetVerbindungEvent = delegate { };

        private DatabaseContentService initServerConnection()
        {
            try
            {
                return new DatabaseContentService();
            }
            catch (WebException)
            {
                return null;
            }
        }
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

        public List<Track> Test()
        {
            List<Track> data = new List<Track> ();

            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\124744.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\124744.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\268452.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\371034.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1176550.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1383637.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1689922.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1936187.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2676847.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2760562.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2786928.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2830496.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2938461.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3012989.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3014395.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3033481.gpx")));
            data.Add (new Track ("125", GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3041433.gpx")));

            return data;
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
                string adresse = "";
                try
                {
                    var addressInfo =
                        await
                            locator.ReverseGeocodeAsync(newPoint.Coordinates, 50, newPoint.Coordinates.SpatialReference,
                                CancellationToken.None);
                    adresse = addressInfo.AddressFields["Address"];

                }
                catch (Exception)
                {
                    Console.WriteLine("Esri Geocode-Server reagiert nicht oder Adresse unbekannt");
                    adresse = "unbekannt";
                }

                HeatPoint heatPoint = null;
                if (_heatPoints.TryGetValue(adresse, out heatPoint))
                {
                    heatPoint.Points.Add(newPoint);
                }
                else
                {
                    _heatPoints.Add(adresse, new HeatPoint(new List<Point>() { newPoint }));
                }

            }
        }

        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            if (_databaseContentService != null)
            {
                try
                {
                //TODO Wenn während das Benutzens das Internet ausfällt, wird hier eine exception geworfen.
                var data = _databaseContentService.GetNewData ();
                var tracks = GpsToEsriParser.ParseJsonToEsriPolyline (data);

                TrackAddedEvent (this, tracks);

                //TrackAddedEvent (this, Test ());

                //var heatPoints = GpsToEsriParser.ParseJsonToPoinList(data);
                //GenerateNewHeatMap(heatPoints);
                //HeatPointAddedEvent(this, heatPoints);
            }
                catch (WebException webException)
                {
                    aTimer.Enabled = false;
                    KeineInternetVerbindungEvent(this, new UnhandledExceptionEventArgs(webException.Status, false));
                }
            }
            else
            {
                aTimer.Enabled = false;
                KeineInternetVerbindungEvent(this, new UnhandledExceptionEventArgs(new WebException("Keine Internetverbindung"), false));
            }
        }



        public void AktiviereUpdate(bool x)
        {
            aTimer.Enabled = x;
        }
    }
}
