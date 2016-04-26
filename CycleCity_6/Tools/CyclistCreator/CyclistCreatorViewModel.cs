using CycleCity_6.Services;
using System.Diagnostics.Contracts;

namespace CycleCity_6.Tools.CyclistCreator
{
    internal class CyclistCreatorViewModel : ToolViewModel
    {
        private readonly CyclistService _cyclistService;

        public CyclistCreatorViewModel(CyclistService cyclistService)
        {
            Contract.Requires(cyclistService != null);

            _cyclistService = cyclistService;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                NotifyPropertyChanged();
            }
        }

        public void AddNewCyclist()
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                _cyclistService.AddNewCyclist(Name);
            }
        }
    }
}
