using Esri.ArcGISRuntime.Geometry;
using System.Diagnostics.Contracts;

namespace CycleCity_6.Materials
{
    internal class Cyclist
    {
        public Cyclist(int id, string name, Polyline track)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(track != null);

            Id = id;
            Name = name;
            Track = track;
        }

        public int Id { get; }

        public string Name { get; }

        public Polyline Track { get; }

        public MapPoint RecentPosition { get; set; }

        public double Distance => CalculateDistance();

        private double CalculateDistance()
        {
            return GeometryEngine.Length(GeometryEngine.Project(Track, SpatialReference.Create(25832)));
        }

        public override bool Equals(object other)
        {
            var otherCyclist = other as Cyclist;

            return otherCyclist != null && Equals(otherCyclist.Id, Id);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
