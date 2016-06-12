using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Tasks.Geocoding;
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
        public static List<Track> ParseGpxToEsriPolyline(string url)
        {
            List<Track> trackList = new List<Track>();
            try
            {
                XDocument gpxDoc = XDocument.Load(@url);
                XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");
                var tracks = from track in gpxDoc.Descendants(gpx + "trk")
                             select new
                             {
                                 Segs = (
                                     from trackpoint in track.Descendants(gpx + "trkpt")
                                     select new
                                     {
                                         Latitude = trackpoint.Attribute("lat").Value,
                                         Longitude = trackpoint.Attribute("lon").Value,
                                     })
                             };

                foreach (var track in tracks)
                {
                    List<MapPoint> points = new List<MapPoint>();
                    foreach (var trekSeg in track.Segs)
                    {
                        var y = Double.Parse(trekSeg.Latitude, CultureInfo.InvariantCulture);
                        var x = Double.Parse(trekSeg.Longitude, CultureInfo.InvariantCulture);
                        points.Add(new MapPoint(x, y));
                    }
                    trackList.Add(new Track("-1",new Polyline(points, SpatialReferences.Wgs84)));
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found");
            }
            return trackList;
        }


        public static List<Track> ParseJsonToEsriPolyline(String json)
        {
            List<Track> trackList = new List<Track>();
            //Hier stürzt beim gleichzeitigen starten das programm ab, weil der token neu vergeben wurde
            JObject jObject = JObject.Parse(json);
            var tracks = jObject.Values();

                var templist = new List<List<MapPoint>>();
            foreach (var track in tracks)
            {
                var id = track.Path;
                var startzeit = getDate((string)track.First["time"]);
                var endzeit = getDate((string)track.Last["time"]);

                var waypoints = track.Children();
                var pointList = waypoints.Select(point => new MapPoint((double)point["lon"], (double)point["lat"], SpatialReferences.Wgs84)).ToList();
                var tour = new Polyline(pointList, SpatialReferences.Wgs84);
                var startpunkt = new Point(pointList.First(), startzeit);
                var endpunkt = new Point(pointList.Last(), endzeit);
                templist.Add(pointList);
                trackList.Add(new Track(id, tour, startpunkt, endpunkt));
            }

            return trackList;
        }

        /// <summary>
        /// Liefert alle Points aus dem Json-String in Form einer Liste von Points
        /// </summary>
        /// <param name="json"></param>
        /// <returns>alle punkte von allen tracks in dem übergebenen json string</returns>
        public static List<Point> ParseJsonToPoinList(string json)
        {

            List<Point> pointList = new List<Point>();
            //Hier stürzt beim gleichzeitigen starten das programm ab, weil der token neu vergeben wurde
            JObject jObject = JObject.Parse(json);
            var tracks = jObject.Values();

            foreach (var track in tracks)
            {
                var waypoints = from points in track.Children()
                                select points;

                foreach (var point in waypoints)
                {
                    //Zeit+Datum für jeden Punkt extrahieren
                    var tempTime = (string)point["time"];
                    var time = getDate(tempTime);
                    var mappoint = new MapPoint((double)point["lon"], (double)point["lat"], (double)point["ele"], SpatialReferences.Wgs84);
                    pointList.Add(new Point(mappoint, time));
                }
            }
            return pointList;
        }

        /// <summary>
        /// Erstellt aus einem Json String ein DateTime-Objekt
        /// </summary>
        /// <param name="timeString">Json String aus dem Element "time"</param>
        /// <returns>Date mit JJ/MM/DD HH/MM/SS</returns>
        private static DateTime getDate(string timeString)
        {
            var tempTime = timeString.Split('\u0020');
            var date = tempTime[0];
            var time = tempTime[1];
            var dateArray = date.Split('-');
            var timeArray = time.Split(':');
            try
            {
                return new DateTime(int.Parse(dateArray[0]), int.Parse(dateArray[1]), int.Parse(dateArray[2]),
                    int.Parse(timeArray[0]), int.Parse(timeArray[1]), int.Parse(timeArray[2]));

            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }
    }
}
