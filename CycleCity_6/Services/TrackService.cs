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

        // Lokale Felder
        private Timer timer;
        private DatabaseContentService databaseContentService;
        private LocalDBService localDBService;

        // Properties
        public List<List<Track>> Velorouten { get; set; }
        public List<Track> AlleDaten { get; private set; }
        public bool Live { get; private set; }

        public TrackService()
        {
            // Server initialisieren
            databaseContentService = initServerConnection();
            localDBService = new LocalDBService ();

            InitTimer ();
            InitVelorouten();

            AlleDaten = HoleDaten (new DateTime (2016, 01, 01, 00, 00, 00), DateTime.Today);
        }

        // Eventhandler
        public event EventHandler<List<Track>> TrackAddedEvent = delegate { };
        public event UnhandledExceptionEventHandler KeineInternetVerbindungEvent = delegate { };

        // Timer initialisieren
        private void InitTimer()
        {
            timer = new Timer (2000);                           // Timer auf 2 Sekunden Intervall eingestellt.
            timer.Elapsed += CollectData_OnTimedEvent;          // Elapsed Event löst die CollectData-Methode aus
            timer.Enabled = false;                              // Timer wird ersteinmal deaktiviert.
        }

        private void InitVelorouten()
        {
            // Velorouten werden geladen
            Velorouten = new List<List<Track>> ();
            for (int i = 1; i < 15; i++)
            {
                Velorouten.Add(GpsToEsriParser.ParseGpxToEsriPolyline(Environment.CurrentDirectory + @"\..\..\" +
                                                          @"\Data\Veloroute_" + i + "_Track.gpx"));
            }

        }

        // Initialisieren der Serverconnection
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

        // Daten werder vom Server abgefragt, verarbeitet und weitergegeben.
        private void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {

            var Json = databaseContentService.GetNewData ();

            if(Json == "[]")
            {
                return;
            }

            var temp = GpsToEsriParser.ParseJsonToEsriPolyline (Json);
            TrackAddedEvent (this, temp);

            //var data = _localDBService.LoadTrackFromDB ("0093ae0aca761f8f6ec5a38600108481");
            //var data = _localDBService.LoadAllTracksFromDB ();

            //var tracks = GpsToEsriParser.JArrayToPolyline (data);
            //tracks.AddRange (Test ());

            //TrackAddedEvent (this, tracks);
            localDBService.AddJson (Json);            
        }

        // Lade Daten entsprechend der gewählten Zeitspanne vom Server
        private List<Track> HoleDaten(DateTime von, DateTime bis)
        {
            var tracks = new List<Track>();
            if (databaseContentService != null)
            {
                try
                {
                    timer.Stop ();
                    var Json = databaseContentService.GetDataFromTo (von, bis);

                    var data = Json;
                    tracks = GpsToEsriParser.ParseJsonToEsriPolyline(data);
                    TrackAddedEvent(this, tracks);

                    localDBService.AddJson (Json);
                }
                catch (WebException webException)
                {
                    timer.Enabled = false;
                    KeineInternetVerbindungEvent(this, new UnhandledExceptionEventArgs(webException.Status, false));
                }
            }
            else
            {
                timer.Enabled = false;
                KeineInternetVerbindungEvent(this, new UnhandledExceptionEventArgs(new WebException("Keine Internetverbindung"), false));
            }
            return tracks;
        }

        // Wähle für die ausgewählte Zeitpanne Tracks aus den Daten vom Server aus 
        public void UpdateVonBis(DateTime von, DateTime bis)
        {
            var tracks = from t in AlleDaten
                         where
                             (t.Startzeit.Month >= von.Month && t.Endzeit.Month <= bis.Month) && (t.Startzeit.Hour >= von.Hour && t.Endzeit.Hour <= bis.Hour)
                         select t;
            TrackAddedEvent (this, tracks.ToList ());

            timer.Stop ();
            //var data = _localDBService.LoadAllTracksFromDB ();

            //var tracks = GpsToEsriParser.JArrayToPolyline (data);
            //tracks.AddRange (Test ());

            //TrackAddedEvent (this, tracks);
        }

        // Startet bzw. Stopt den Timer 
        public void AktiviereLiveUpdate(bool x)
        {
            if(x)
            {
                timer.Start ();
            }
            else
            {
                timer.Stop ();
            }

        }
    }
}
