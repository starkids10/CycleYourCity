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
            catch(FileNotFoundException)
                {
                Console.WriteLine ("File not found");
                points = new List<MapPoint> ();
                }
            return new Polyline (points, SpatialReferences.Wgs84);
        }


        public static List<Track> ParseJsonToEsriPolyline(String json)
        {
            List<MapPoint> pointList = new List<MapPoint>();
            List<Track> trackList = new List<Track>();
            JArray jArray = JArray.Parse(json);

            foreach(var track in jArray)
            {
                var id = (int)track["tourid"];
                var start = getDate((string)track["WayPoints"].First["time"]);
                var end = getDate((string)track["WayPoints"].Last["time"]);

                var waypoints = from points in track["WayPoints"].Children ()
                    select points;
                foreach(var point in waypoints)
                {
                    pointList.Add(new MapPoint((double) point["lat"], (double) point["lon"]));
                }
                var tour = new Polyline(pointList, SpatialReferences.Wgs84);
                trackList.Add(new Track(id,tour,start,end));
             }
            //Debug
            foreach (var track in trackList)
            {
                Console.WriteLine(track.Startzeit + "\n" + track.Endzeit + "\n" + track.Tour);
            }

            return trackList;
        }

        /// <summary>
        /// Liefert alle Points aus dem Json-String in Form einer Liste von Points
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<Point> ParseJsonToPoinList(string json)
        {

            List<Point> pointList = new List<Point>();
            JObject jObject = JObject.Parse(json);
            var tracks = jObject["tracks"];
            
            foreach (var track in tracks)
            {
                var waypoints = from points in track["WayPoints"].Children()
                                select points;
                foreach (var point in waypoints)
                {
                    //Zeit+Datum für jeden Punkt extrahieren
                    var tempTime = (string)point["time"];
                    var time = getDate(tempTime);
                    var mappoint = new MapPoint((double) point["lat"], (double) point["lon"],0,SpatialReferences.Wgs84);
                    pointList.Add(new Point(mappoint,time));
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
            return new DateTime(int.Parse(dateArray[0]), int.Parse(dateArray[1]), int.Parse(dateArray[2]),
                int.Parse(timeArray[0]), int.Parse(timeArray[1]), int.Parse(timeArray[2]));
        }
    }
}
