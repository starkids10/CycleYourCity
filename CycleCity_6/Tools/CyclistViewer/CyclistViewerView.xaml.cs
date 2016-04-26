using Esri.ArcGISRuntime.Layers;
using System.Diagnostics.Contracts;
using System.Windows.Controls;

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

            GetViewModel().MapLayer = (GraphicsLayer)MapView.Map.Layers["CyclistLayer"];
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
                MapView.SetView(GetViewModel().SelectedCyclist.Track);
            }
        }
    }
}
