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
        private readonly Dictionary<string, HeatPoint> _heatPoints;
        private Timer aTimer;
        private readonly DatabaseContentService _databaseContentService;

        public DateTime Startzeit { get; set; }
        public DateTime Endzeit { get; set; }
        public bool HeatmapAnzeigen { get; set; }
        public List<Track> Velorouten { get; set; }

        public TrackService()
        {
            _databaseContentService = initServerConnection();
            _heatPoints = new Dictionary<string, HeatPoint>();
            HeatmapAnzeigen = false;

            aTimer = new Timer(10000);
            aTimer.Elapsed += CollectData_OnTimedEvent;
            aTimer.Enabled = true;

            Startzeit = new DateTime(2016,05,20,00,00,00);
            Endzeit = DateTime.Now;

            Velorouten = GpsToEsriParser.ParseGpxToEsriPolyline(Environment.CurrentDirectory + @"\..\..\"  + @"\Data\Velorouten_Hamburg.gpx");
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

        public IEnumerable<HeatPoint> GetAllHeatPoints()
        {
            Contract.Ensures(Contract.Result<IEnumerable<HeatPoint>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<HeatPoint>>().Any());
            return _heatPoints.Values;
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

            }
        }

        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            if (_databaseContentService != null)
            {
                try
                {
                    var data = _databaseContentService.GetDataFromTo(Startzeit, Endzeit);
                    if (HeatmapAnzeigen)
                    {
                        var heatPoints = GpsToEsriParser.ParseJsonToPoinList(data);
                        GenerateNewHeatMap(heatPoints);
                        HeatPointAddedEvent(this, _heatPoints.Values);
                    }
                    else
                    {
                        var tracks = GpsToEsriParser.ParseJsonToEsriPolyline(data);
                        //TrackAddedEvent(this, tracks);
                        TrackAddedEvent (this, Test ());

                    }


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
