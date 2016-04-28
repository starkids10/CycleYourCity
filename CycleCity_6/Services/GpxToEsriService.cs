using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Services
{
    public class GpxToEsriService
    {
        public static Polyline parseGPXtoEsri(string url)
        {
            XDocument gpxDoc = XDocument.Load (@url);
            XNamespace gpx = XNamespace.Get ("http://www.topografix.com/GPX/1/1");
            List<MapPoint> points = new List<MapPoint> ();
            var tracks = from track in gpxDoc.Descendants (gpx + "trk")
                         select new
                         {
                             Segs = (
                                from trackpoint in track.Descendants (gpx + "trkpt")
                                select new
                                {
                                    Latitude = trackpoint.Attribute ("lat").Value,
                                    Longitude = trackpoint.Attribute ("lon").Value,
                                }
                              )
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
            return new Polyline (points, SpatialReferences.Wgs84);
        }
    }
}
