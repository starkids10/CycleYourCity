using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;

namespace CycleCity_6.Tools.CyclistCreator
{
    /// <summary>
    /// Interaktionslogik für CyclistCreatorView.xaml
    /// </summary>
    public partial class CyclistCreatorView : UserControl
    {
        public CyclistCreatorView()
        {
            InitializeComponent();
        }

        private CyclistCreatorViewModel GetViewModel()
        {
            Contract.Requires(DataContext is CyclistCreatorViewModel);

            return (CyclistCreatorViewModel) DataContext;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            GetViewModel().AddNewCyclist();
        }
    }
}
