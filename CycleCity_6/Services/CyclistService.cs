using CycleCity_6.Materials;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CycleCity_6.Services
{
    internal class CyclistService
    {

        private readonly List<Cyclist> _cyclists;

        private int _idCounter = 0;


        public CyclistService()
        {
            _cyclists = new List<Cyclist>();
        }

        public event EventHandler<Cyclist> CyclistAddedEvent = delegate { };

        /// <summary>
        /// Returns all cyclists.
        /// </summary>
        /// <returns>all registered cyclists</returns>
        public IEnumerable<Cyclist> GetAllCyclists()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Cyclist>>() != null);
            Contract.Ensures(Contract.Result<IEnumerable<Cyclist>>().Any());

            return _cyclists;
        }

        /// <summary>
        /// Adds a new Cyclist to the list
        /// </summary>
        /// <param name="name">name of the cyclist</param>
        public void AddNewCyclist(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            var newCyclist = new Cyclist(_idCounter++, name,
                new Polyline(new[] {new MapPoint(0, 0)}, SpatialReferences.Wgs84));

            _cyclists.Add(newCyclist);

            CyclistAddedEvent(this, newCyclist);
        }

        /// <summary>
        /// Adds a new Cyclists and his track to the list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        public void AddNewCyclist(string name, string url)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(!string.IsNullOrWhiteSpace(url));

            Polyline track = GpsToEsriParser.ParseGpxToEsriPolyline(url);

            var newCyclist = new Cyclist(_idCounter++, name, track);

            _cyclists.Add(newCyclist);

            CyclistAddedEvent(this, newCyclist);
        }
    }
}
