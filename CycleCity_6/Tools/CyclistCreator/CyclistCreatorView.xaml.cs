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

        // TODO  schön machen wenn Zeit ist ("URL" und "Name" wieder einblenden)
        private void UrlText_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (UrlTextbox.Text.Equals("URL"))
            {
                UrlTextbox.Text = "";
            }
        }

        private void NameText_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (NameTextbox.Text.Equals("Name"))
            {
                NameTextbox.Text = "";
            }
        }
    }
}
