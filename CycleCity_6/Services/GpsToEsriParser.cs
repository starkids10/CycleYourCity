using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using Newtonsoft.Json;
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


        //TODO Parser muss eine Liste von Polylines zurückgeben da mehrere tracks änderungen in dem String übergenen werden könnten.
        public static Polyline ParseJsonToEsriPolyline(String json)
        {
            List<MapPoint> pointList = new List<MapPoint> ();
            JObject jObject = JObject.Parse (json);

            var Id = (int)jObject["tourid"];
            var waypoints = from points in jObject["WayPoints"].Children ()
                            select points;

            foreach(var point in waypoints)
            {
                pointList.Add (new MapPoint ((double)point["lat"], (double)point["lon"]));
            }
            return new Polyline (pointList, SpatialReferences.Wgs84);
        }
    }
}
