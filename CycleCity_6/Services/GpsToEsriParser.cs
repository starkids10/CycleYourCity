using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using Newtonsoft.Json.Linq;

namespace CycleCity_6.Services
{
    public class GpsToEsriParser
    {
        /// <summary>
        /// Parses the tracks from a .gpx file to a Esri Polyline Object, which can be displayed on 
        /// </summary>
        /// <param name="url">Path to .gpx file</param>
        /// <returns>represents the track in the gpx file as a polyline</returns>
        public static Polyline ParseGpxToEsriPolyline(string url)
        {
            List<MapPoint> points = new List<MapPoint> ();
            try
            {
                XDocument gpxDoc = XDocument.Load (@url);
                XNamespace gpx = XNamespace.Get ("http://www.topografix.com/GPX/1/1");
                var tracks = from track in gpxDoc.Descendants (gpx + "trk")
                                select new
                                {
                                    Segs = (
                                        from trackpoint in track.Descendants (gpx + "trkpt")
                                        select new
                                        {
                                    Latitude = trackpoint.Attribute ("lat").Value,
                                    Longitude = trackpoint.Attribute ("lon").Value,
                                        })
                                };

                foreach(var track in tracks)
                {
                    foreach(var trekSeg in track.Segs)
                    {
                        var y = Double.Parse (trekSeg.Latitude, CultureInfo.InvariantCulture);
                        var x = Double.Parse (trekSeg.Longitude, CultureInfo.InvariantCulture);
                        points.Add (new MapPoint (x, y));
                    }
                }
                Console.WriteLine (points.Count);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found");
                    points = new List<MapPoint>();
                }
            return new Polyline (points, SpatialReferences.Wgs84);
        }


        //TODO Parser muss eine Liste von Polylines zurückgeben da mehrere tracks änderungen in dem String übergenen werden könnten.
        public static List<Track> ParseJsonToEsriPolyline(String json)
        {
            List<MapPoint> pointList = new List<MapPoint>();
            JObject jObject = JObject.Parse(json);

            var Id = (int) jObject["tourid"];
            //TODO Startzeit und Endzeit vom ersten/letzten Punkt übertragen
            //Casten zu DateTime von String geht nicht -> string zu mehreren Ints parsen oder json ändern
            //var startzeit = (DateTime) jObject["WayPoints"].First["time"];
            //var endzeit = (DateTime) jObject["WayPoints"].Last["time"];
            var waypoints = from points in jObject["WayPoints"].Children()
                select points;

            foreach (var point in waypoints)
            {
                //Zeit+Datum für jeden Punkt extrahieren
                var tempTime = (string) point["time"];
                var time = getDate(tempTime);
                pointList.Add(new MapPoint((double) point["lat"], (double) point["lon"]));
            }
            var tour = new Polyline(pointList, SpatialReferences.Wgs84);


            return new List<Track> {new Track(Id, tour, new DateTime(), new DateTime())};
        }

        private static DateTime getDate(string timeString)
        {
            var tempTime = timeString.Split(' ');
            var date = tempTime[0];
            var time = tempTime[1];
            var dateArray = date.Split('-');
            var timeArray = time.Split(':');
            return new DateTime(int.Parse(dateArray[0]), int.Parse(dateArray[1]), int.Parse(dateArray[2]),
                int.Parse(timeArray[0]), int.Parse(timeArray[1]), int.Parse(timeArray[2]));
        }
    }
}
