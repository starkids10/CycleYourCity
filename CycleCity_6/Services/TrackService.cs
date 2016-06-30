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
        private LocalDBService _localDBService;

        public List<List<Track>> Velorouten { get; set; }
        public List<Track> AlleDaten;

        public TrackService()
        {
            _databaseContentService = initServerConnection();
            _localDBService = new LocalDBService ();

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

        private DatabaseContentService initServerConnection()
        {
            try
            {
                return new DatabaseContentService ();
            }
            catch(WebException)
            {
                return null;
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

        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            var Json = _databaseContentService.GetNewData ();

            var temp = GpsToEsriParser.ParseJsonToEsriPolyline (Json);
            TrackAddedEvent (this, temp);


            //var data = _localDBService.LoadTrackFromDB ("0093ae0aca761f8f6ec5a38600108481");
            //var data = _localDBService.LoadAllTracksFromDB ();

            //var line = GpsToEsriParser.JArrayToPolyline (data);

            //TrackAddedEvent (this, line);
            _localDBService.AddJson (Json);
        }

        private List<Track> HoleDaten(DateTime von, DateTime bis)
        {
            var tracks = new List<Track>();
            if (_databaseContentService != null)
            {
                try
                {
                    var Json = _databaseContentService.GetDataFromTo (von, bis);

                    var data = Json;
                    tracks = GpsToEsriParser.ParseJsonToEsriPolyline(data);
                    TrackAddedEvent(this, tracks);

                    _localDBService.AddJson (Json);
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
