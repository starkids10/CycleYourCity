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
        public List<Track> alleDaten;

        public TrackService()
        {
            _databaseContentService = initServerConnection();

            _aTimer = new Timer(10000);
            _aTimer.Elapsed += CollectData_OnTimedEvent;
            _aTimer.Enabled = false;

            Velorouten = new List<List<Track>>();
            InitVelorouten();
            ;
            alleDaten = HoleDaten(new DateTime(2016, 01, 01, 00, 00, 00), DateTime.Today);
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
                return new DatabaseContentService();
            }
            catch (WebException)
            {
                return null;
            }
        }


        //public List<Track> Test()
        //{
        //    List<Track> data = new List<Track>();

        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\124744.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\124744.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\268452.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\371034.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\1176550.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\1383637.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\1689922.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\1936187.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\2676847.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\2760562.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\2786928.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\2830496.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\2938461.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\3012989.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\3014395.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\3033481.gpx")));
        //    data.Add(new Track("125", GpsToEsriParser.ParseGpxToEsriPolyline(@"C:\Users\David\Desktop\3041433.gpx")));

        //    return data;
        //}

        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            HoleDaten(new DateTime(2016, 01, 01, 00, 00, 00), DateTime.MaxValue);
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
            var tracks = from t in alleDaten
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
