using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Materials
{
    public class Track
    {
        public Track(int id, Polyline tour)
        {
            Contract.Requires (tour != null);

            Id = id;
            Tour = tour;
        }

        public Track(int id, Polyline tour, DateTime start, DateTime end)
            : this(id, tour)
        {
            Contract.Requires(start != null);
            Contract.Requires(end != null);

            Startzeit = start;
            Endzeit = end;
        }



        public int Id { get; }
        public Polyline Tour { get; set; }
        public DateTime Startzeit { get; }
        public DateTime Endzeit { get; set; }

        public double Distance => CalculateDistance ();

        private double CalculateDistance()
        {
            return GeometryEngine.Length (GeometryEngine.Project (Tour, SpatialReference.Create (25832)));
        }

        public override bool Equals(object other)
        {
            var otherTrack = other as Track;

            return otherTrack != null && Equals (otherTrack.Id, Id);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
