using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleCity_6.Materials
{
    public class HeatPoint
    {
        public HeatPoint(List<Point> points)
        {
            Points = points;
           
        }

        public List<Point> Points { get; private set; }
        public int Heat { get { return Points.Count; } }

        public void AddNewPoint(Point point)
        {
            Points.Add(point);

        }

        public void AddNewPoints(List<Point> points)
        {
            Points = Points.Concat(points).ToList();
        }
    }
}
