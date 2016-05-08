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

        private void CyclistListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetViewModel().HasSelectedCyclist())
            {
                var cyclist = GetViewModel().SelectedCyclist;
                CycleMapView.SetView(cyclist.Track.Tour);
            }
        }

        private void CycleMapView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var z = e.GetPosition (this);
            var mappoint = CycleMapView.ScreenToLocation (z);

            Console.WriteLine ("" + mappoint.X + " " + mappoint.Y + " " + mappoint.Z);
        }
    }
}
