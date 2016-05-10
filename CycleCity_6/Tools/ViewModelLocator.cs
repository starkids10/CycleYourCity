using CycleCity_6.Services;
using CycleCity_6.Tools.CyclistViewer;

namespace CycleCity_6.Tools
{
    internal class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var cyclistService = new TrackService();

            CyclistViewerViewModel = new CyclistViewerViewModel(cyclistService);
        }

        public CyclistViewerViewModel CyclistViewerViewModel { get; }

    }
}
