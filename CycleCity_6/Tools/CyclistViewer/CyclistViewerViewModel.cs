﻿using System;
using CycleCity_6.Materials;
using CycleCity_6.Services;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Timers;
using System.Windows.Media;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : ToolViewModel
    {
        private Timer aTimer;

        public CyclistViewerViewModel(CyclistService cyclistService)
        {
            Contract.Requires(cyclistService != null);

            Cyclists = new ObservableCollection<Cyclist>(cyclistService.GetAllCyclists());

            cyclistService.CyclistAddedEvent += CyclistService_OnCyclistAdded;

            aTimer = new Timer(1000);
            aTimer.Elapsed += CollectData_OnTimedEvent;
            aTimer.Enabled = true;
        }

        public ObservableCollection<Cyclist> Cyclists { get; }

        private Cyclist _selectedCyclist;
        public Cyclist SelectedCyclist
        {
            get { return _selectedCyclist; }
            set
            {
                _selectedCyclist = value;

                NotifyPropertyChanged();
            }
        }

        public bool HasSelectedCyclist()
        {
            return _selectedCyclist != null;
        }

        private GraphicsLayer _mapLayer;
        public GraphicsLayer MapLayer
        {
            get
            {
                Contract.Requires(HasMapLayer());

                return _mapLayer;
            }
            set
            {
                Contract.Requires(value != null);
                Contract.Ensures(HasMapLayer());

                _mapLayer = value;

                AddCyclistsToMapLayer(value);
            }
        }

        public bool HasMapLayer()
        {
            return _mapLayer != null;
        }

        private void AddCyclistsToMapLayer(GraphicsLayer mapLayer)
        {
            Contract.Requires(mapLayer != null);

            foreach (var cyclist in Cyclists)
            {
                AddCyclistToMapLayer(mapLayer, cyclist);
            }
        }

        /// <summary>
        /// adds new track to map and generates the color of the track
        /// </summary>
        /// <param name="mapLayer"></param>
        /// <param name="cyclist"></param>
        private static void AddCyclistToMapLayer(GraphicsLayer mapLayer, Cyclist cyclist)
        {
            Contract.Requires(mapLayer != null);
            Contract.Requires(cyclist != null);
            var simpleLineSymbol = new SimpleLineSymbol();
            simpleLineSymbol.Width = 3;
            Random randomGen = new Random();
            var randomColor = Color.FromRgb((byte)randomGen.Next(255), (byte)randomGen.Next(255),
                (byte)randomGen.Next(255));
            simpleLineSymbol.Color = randomColor;
            mapLayer.Graphics.Add(new Graphic(cyclist.Track.Tour, simpleLineSymbol));
        }

        private void CyclistService_OnCyclistAdded(object sender, Cyclist newCyclist)
        {
            Contract.Requires(newCyclist != null);

            Cyclists.Add(newCyclist);

            if (HasMapLayer())
            {
                AddCyclistToMapLayer(MapLayer, newCyclist);
            }
        }

        //TODO erst einkommentieren wenn wir daten vom Server kriegen
        private  void CollectData_OnTimedEvent(Object souce, System.Timers.ElapsedEventArgs e)
        {
            /*
            string data = DatabaseContentService.GetNewData();

            Polyline newTrack = GpsToEsriParser.ParseJsonToEsriPolyline(data);

            // TODO Auflösung der Cyclist --> umstellung auf Track??
            Cyclist newCyclist = new Cyclist(0, "", newTrack);
            if (HasMapLayer())
            {
                AddCyclistToMapLayer(MapLayer, newCyclist);
            }
            */
        }
    }
}
