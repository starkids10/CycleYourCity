using Esri.ArcGISRuntime.Geometry;
using System;


namespace CycleCity_6.Materials
{
    public class Point
    {
        public Point(MapPoint point, DateTime time)
        {
            Coordinates = point;
            Time = time;
        }
        public MapPoint Coordinates { get; }
        public DateTime Time { get; }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point point = (Point)obj;
                return this.Coordinates.Equals(point.Coordinates) && this.Time.Equals(point.Time);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Time.Second * Time.Minute * Time.Hour;
        }
    }
}
