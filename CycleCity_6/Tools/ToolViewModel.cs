using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CycleCity_6.Tools
{
    internal abstract class ToolViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
