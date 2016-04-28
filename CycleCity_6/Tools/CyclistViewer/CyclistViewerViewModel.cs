using CycleCity_6.Materials;
using CycleCity_6.Services;
using Esri.ArcGISRuntime.Layers;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace CycleCity_6.Tools.CyclistViewer
{
    internal class CyclistViewerViewModel : ToolViewModel
    {
        public CyclistViewerViewModel(CyclistService cyclistService)
        {
            Contract.Requires(cyclistService != null);

            Cyclists = new ObservableCollection<Cyclist>(cyclistService.GetAllCyclists());

            cyclistService.CyclistAddedEvent += CyclistService_OnCyclistAdded;
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

        private static void AddCyclistToMapLayer(GraphicsLayer mapLayer, Cyclist cyclist)
        {
            Contract.Requires(mapLayer != null);
            Contract.Requires(cyclist != null);

            mapLayer.Graphics.Add(new Graphic(cyclist.Track));
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

       /* public void DummyTrack()
        {
            MapLayer.Graphics.Add (new Graphic (GpxToEsriService.parseGPXtoEsri ()));
        }*/
    }
}
