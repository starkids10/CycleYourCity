using CycleCity_6.Services;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

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

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyPropertyChanged();
            }
        }

        public void AddNewCyclist()
        {
            Regex regex = new Regex(@"\S*.gpx");
            Match match = regex.Match(Url);
            if (!string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(Name) && match.Success)
            {
                _cyclistService.AddNewCyclist(Name, Url);
            }
            else if (!string.IsNullOrWhiteSpace(Name))
            {
                _cyclistService.AddNewCyclist(Name);
            }
            else if (!string.IsNullOrWhiteSpace(Url) && match.Success)
            {
                _cyclistService.AddNewCyclist("Unnamed", Url);
            }

        }
    }
}
