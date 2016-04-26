using CycleCity_6.Services;
using CycleCity_6.Tools.CyclistCreator;
using CycleCity_6.Tools.CyclistViewer;

namespace CycleCity_6.Tools
{
    internal class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var cyclistService = new CyclistService();

            CyclistViewerViewModel = new CyclistViewerViewModel(cyclistService);
            CyclistCreatorViewModel = new CyclistCreatorViewModel(cyclistService);
        }

        public CyclistViewerViewModel CyclistViewerViewModel { get; }

        public CyclistCreatorViewModel CyclistCreatorViewModel { get; }
    }
}
