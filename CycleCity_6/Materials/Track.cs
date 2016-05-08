using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Materials
{
    public class Track
    {
        public int Id { get;}
        public Polyline Tour{ get;}
        public DateTime Startzeit { get; }
        public DateTime Endzeit { get;  }

        public Track(Polyline tour)
        {
            Id = 0;
            Tour = tour;
            Startzeit = new DateTime();
            Endzeit = new DateTime();
        }

        public Track(int id, Polyline tour, DateTime start, DateTime ende)
    {
            Id = id;
            Tour = tour;
            Startzeit = start;
            Endzeit = ende;
        }
    }
}
