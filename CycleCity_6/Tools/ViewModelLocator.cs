using System;
using CycleCity_6.Services;
using CycleCity_6.Tools.CyclistViewer;

namespace CycleCity_6.Tools
{
    internal class ViewModelLocator
    {
        public ViewModelLocator()
        {

            var trackService = new TrackService();
            var localDBService = new LocalDBService ();

            CyclistViewerViewModel = new CyclistViewerViewModel(trackService, localDBService);
        }

        public CyclistViewerViewModel CyclistViewerViewModel { get; }

    }
}
