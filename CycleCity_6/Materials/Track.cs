using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Materials
{
    internal class Track
    {
        public int Id { get; set; }
        public Polyline  Tour{ get; set; }
    }
}
