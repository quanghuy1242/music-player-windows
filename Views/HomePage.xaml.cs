using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MPA.Views
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private void OnExploreClicked(object sender, RoutedEventArgs e)
        {
            // Navigate to the Albums page
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(AlbumPage));
            }
        }
    }
}