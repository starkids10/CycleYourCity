﻿using System;
using System.Diagnostics.Contracts;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Materials
{
    [Serializable]
    public class Track
    {
        public Track(string id, Polyline tour, Point start, Point ende)
        {
            Contract.Requires (tour != null);

            Id = id;
            Tour = tour;
            Startpunkt = start;
            Endpunkt = ende;
            Startzeit = start.Time;
            Endzeit = ende.Time;
        }

        public Track(string id, Polyline tour)
        {
            Contract.Requires (tour != null);

            Id = id;
            Tour = tour;
            Startpunkt = null;
            Endpunkt = null;
            Startzeit = DateTime.Now;
            Endzeit = DateTime.Now;
        }

        public string Id { get; }
        public Polyline Tour { get; set; }
        public Point Startpunkt { get; set; }
        public Point Endpunkt { get; set; }
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
            return Id.Length;
        }
    }
}
