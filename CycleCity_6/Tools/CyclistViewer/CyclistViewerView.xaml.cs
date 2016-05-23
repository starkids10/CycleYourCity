using Esri.ArcGISRuntime.Layers;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Tasks.Geocoding;

namespace CycleCity_6.Tools.CyclistViewer
{
    /// <summary>
    /// Interaktionslogik für CyclistViewerView.xaml
    /// </summary>
    public partial class CyclistViewerView : UserControl
    {
        public CyclistViewerView()
        {
            InitializeComponent ();

            GetViewModel ().MapLayer = (GraphicsLayer)CycleMapView.Map.Layers["CyclistLayer"];
            GetViewModel ().GraphicsCollection += DisplayGraphics;
        }

        private CyclistViewerViewModel GetViewModel()
        {
            Contract.Requires (DataContext is CyclistViewerViewModel);

            return (CyclistViewerViewModel)DataContext;
        }

        private void DisplayGraphics(object sender, List<Graphic> graphics)
        {
                var graphicsLayer = CycleMap.Layers["CyclistLayer"] as Esri.ArcGISRuntime.Layers.GraphicsLayer;

                foreach(var graphic in graphics)
                {
                    graphicsLayer.Graphics.Add (graphic);
                }            
        }

        private void HeatMapOrTracksAnzeigen_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
