using Esri.ArcGISRuntime.Layers;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using System;

namespace CycleCity_6.Tools.CyclistViewer
{
    /// <summary>
    /// Interaktionslogik für CyclistViewerView.xaml
    /// </summary>
    public partial class CyclistViewerView : UserControl
    {
        public CyclistViewerView()
        {
            InitializeComponent();

            GetViewModel().MapLayer = (GraphicsLayer)CycleMapView.Map.Layers["CyclistLayer"];
        }

        private CyclistViewerViewModel GetViewModel()
        {
            Contract.Requires(DataContext is CyclistViewerViewModel);

            return (CyclistViewerViewModel)DataContext;
        }
    }
}
