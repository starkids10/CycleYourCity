using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Materials
{
    // Test mergekonfikt
    internal class Track
    {
        public Track(int id, Polyline tour)
        {
            Contract.Requires (tour != null);

            Id = id;
            Tour = tour;
        }

        public int Id { get; }
        public Polyline Tour { get; set; }

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
