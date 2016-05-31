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

        public event EventHandler<Track> TrackAddedEvent = delegate { };
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
                    var data = _databaseContentService.GetNewData();
                    var tracks = GpsToEsriParser.ParseJsonToEsriPolyline(data);
                    foreach (Track track in tracks)
                    {
                        TrackAddedEvent(this, track);
                    }

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
