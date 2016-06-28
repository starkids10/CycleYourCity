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
        private readonly Timer _aTimer;
        private readonly DatabaseContentService _databaseContentService;
        public List<List<Track>> Velorouten { get; set; }
        public List<Track> AlleDaten;

        public TrackService()
        {
            _databaseContentService = initServerConnection();

            _aTimer = new Timer(2000);
            _aTimer.Elapsed += CollectData_OnTimedEvent;
            _aTimer.Enabled = false;

            Velorouten = new List<List<Track>>();
            InitVelorouten();
            
            AlleDaten = HoleDaten(new DateTime(2016, 01, 01, 00, 00, 00), DateTime.Today);
        }

        public event EventHandler<List<Track>> TrackAddedEvent = delegate { };
        public event UnhandledExceptionEventHandler KeineInternetVerbindungEvent = delegate { };

        private void InitVelorouten()
        {
            for (int i = 1; i < 15; i++)
            {
                Velorouten.Add(GpsToEsriParser.ParseGpxToEsriPolyline(Environment.CurrentDirectory + @"\..\..\" +
                                                          @"\Data\Veloroute_" + i + "_Track.gpx"));
            }

        }


        public List<Track> Test()
        {
            List<Track> data = new List<Track> ();

            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\124744.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\124744.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\268452.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\371034.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1176550.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1383637.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1689922.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\1936187.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2676847.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2760562.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2786928.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2830496.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\2938461.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3012989.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3014395.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3033481.gpx").First());
            data.Add (GpsToEsriParser.ParseGpxToEsriPolyline (@"C:\Users\David\Desktop\3041433.gpx").First());

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

        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            var temp = GpsToEsriParser.ParseJsonToEsriPolyline(_databaseContentService.GetNewData());
            TrackAddedEvent(this, temp);
        }

        private List<Track> HoleDaten(DateTime von, DateTime bis)
        {
            var tracks = new List<Track>();
            if (_databaseContentService != null)
            {
                try
                {
                    var data = _databaseContentService.GetDataFromTo(von, bis);
                    tracks = GpsToEsriParser.ParseJsonToEsriPolyline(data);
                    TrackAddedEvent(this, tracks);
                }
                catch (WebException webException)
                {
                    _aTimer.Enabled = false;
                    KeineInternetVerbindungEvent(this, new UnhandledExceptionEventArgs(webException.Status, false));
                }
            }
            else
            {
                _aTimer.Enabled = false;
                KeineInternetVerbindungEvent(this, new UnhandledExceptionEventArgs(new WebException("Keine Internetverbindung"), false));
            }
            return tracks;
        }

        public void UpdateVonBis(DateTime von, DateTime bis)
        {
            var tracks = from t in AlleDaten
                where
                    (t.Startzeit.Month >= von.Month && t.Endzeit.Month <= bis.Month) && (t.Startzeit.Hour >= von.Hour && t.Endzeit.Hour <= bis.Hour)
                select t;
            TrackAddedEvent(this, tracks.ToList());
        }

        public void AktiviereLiveUpdate(bool x)
        {
            _aTimer.Enabled = x;
        }
    }
}
