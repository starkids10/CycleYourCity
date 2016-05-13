using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleCity_6.Materials
{
    class Point
    {
        public Point(MapPoint point, DateTime time)
        {
            Coordinates = point;
            Time = time;
        }
        MapPoint Coordinates { get; }
        DateTime Time { get; }

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
