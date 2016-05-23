using Esri.ArcGISRuntime.Layers;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System.Windows.Media;

namespace CycleCity_6.Tools.CyclistViewer
{
    /// <summary>
    /// Interaktionslogik für CyclistViewerView.xaml
    /// </summary>
    public partial class CyclistViewerView : UserControl
    {

        GraphicsLayer gLayer;

        public CyclistViewerView()
        {
            InitializeComponent ();

            gLayer = (GraphicsLayer)CycleMapView.Map.Layers["CyclistLayer"];

            GetViewModel ().MapLayer = gLayer;
            GetViewModel ().GraphicsCollection += DisplayGraphics;
        }

        private CyclistViewerViewModel GetViewModel()
        {
            Contract.Requires (DataContext is CyclistViewerViewModel);

            return (CyclistViewerViewModel)DataContext;
        }

        private void DisplayGraphics(object sender, List<Graphic> graphics)
        {
            //gLayer.Graphics.AddRange (graphics);

            // löscht alle vorherigen elemente von dem graphicslayer
            this.CycleMapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.Clear ());
            // Zeichnet die neue Graphics Collection auf den graphicslayer 
            this.CycleMapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.AddRange (graphics));

            var mapPoint = new MapPoint (1091513, 7102386);
            var markerSym = new Esri.ArcGISRuntime.Symbology.SimpleMarkerSymbol ();
            markerSym.Color = Colors.Red;
            markerSym.Style = Esri.ArcGISRuntime.Symbology.SimpleMarkerStyle.Circle;
            markerSym.Size = 5;

            var pointGraphic = new Esri.ArcGISRuntime.Layers.Graphic ();
            pointGraphic.Geometry = mapPoint;
            pointGraphic.Symbol = markerSym;
            this.CycleMapView.Dispatcher.InvokeAsync (() => gLayer.Graphics.Add (pointGraphic));
        }

        private void HeatMapOrTracksAnzeigen_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
