using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CycleCity_6.Services
{
    internal class TrackService
    {

        private readonly List<Track> _tracks;

        public TrackService()
        {
            _tracks = new List<Track>();
        }

        public event EventHandler<Track> TrackAddedEvent = delegate { };

        /// <summary>
        /// Returns all cyclists.
        /// </summary>
        /// <returns>all registered cyclists</returns>
        public IEnumerable<Track> GetAllTracks()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Track>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<Track>>().Any());

            return _tracks;
        }

        /// <summary>
        /// Adds a new Tour to the list
        /// </summary>
        /// <param name="id">id of the track</param>
        public void AddNewTrack(int id)
        {
            var newTrack = new Track(id,
                new Polyline(new[] {new MapPoint(0, 0)}, SpatialReferences.Wgs84));

            _tracks.Add(newTrack);

            TrackAddedEvent(this, newTrack);
        }

        /// <summary>
        /// Adds a new Tour to the list
        /// </summary>
        /// <param name="id">id of the track</param>
        /// <param tour="tour">Polyline foor the track wich simulate the tour</param>
        public void AddNewTrack(int id, Polyline tour)
        {

            var newTrack = new Track(id, tour);

            _tracks.Add(newTrack);

            TrackAddedEvent(this, newTrack);
        }
    }
}
