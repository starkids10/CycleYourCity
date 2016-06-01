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

                var x1 = this.Coordinates.X;
                var x2 = point.Coordinates.X;
                var y1 = this.Coordinates.Y;
                var y2 = point.Coordinates.Y;
                return x1 == x2 && y1 == y2 ;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Time.Second * Time.Minute * Time.Hour;
        }
    }
}
