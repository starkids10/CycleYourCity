using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleCity_6.Materials
{
    public class HeatPoint
    {
        private int heat;

        public HeatPoint(string location, List<Point> points)
        {
            Location = location;
            Points = points;
            heat = Points.Count;
        }

        public String Location { get; }
        public List<Point> Points { get; }

        public int Heat
        {
            get { return heat; } 
        }
    }
}
