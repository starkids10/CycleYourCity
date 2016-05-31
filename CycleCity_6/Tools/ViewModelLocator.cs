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
                CyclistViewerViewModel = new CyclistViewerViewModel(trackService);
        }

        public CyclistViewerViewModel CyclistViewerViewModel { get; }

    }
}
