using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleCity_6.Materials
{
    public class HeatPoint
    {
        private int _heat ;

        public HeatPoint(List<Point> points)
        {
            Points = points;
            _heat = Points.Count;
        }

        public void AddNewPoint(Point point)
        {
            Points.Add(point);
            _heat = Points.Count;
        }

        public void AddNewPoints(List<Point> points)
        {
            Points.Concat(points);
            _heat = Points.Count;
        }

        public List<Point> Points { get; }
        public int Heat { get { return _heat; } }
    }
}
