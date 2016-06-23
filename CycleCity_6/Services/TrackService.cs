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

            _aTimer = new Timer(10000);
            _aTimer.Elapsed += CollectData_OnTimedEvent;
            _aTimer.Enabled = false;

            Velorouten = new List<List<Track>>();
            InitVelorouten();
            ;
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
                return new DatabaseContentService();
            }
            catch (WebException)
            {
                return null;
            }
        }

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
