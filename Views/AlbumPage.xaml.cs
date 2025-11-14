using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MPA.Views
{
    public sealed partial class AlbumPage : Page
    {
        private int previousSelectedIndex = 1; // Start with Albums (index 1)

        public AlbumPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(AlbumsPage));
        }

        private void NavigationSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectedItem = sender.SelectedItem;
            int currentSelectedIndex = sender.Items.IndexOf(selectedItem);
            System.Type pageType;

            switch (currentSelectedIndex)
            {
                case 0:
                    pageType = typeof(SongsPage);
                    break;
                case 1:
                    pageType = typeof(AlbumsPage);
                    break;
                case 2:
                    pageType = typeof(ArtistsPage);
                    break;
                default:
                    pageType = typeof(AlbumsPage);
                    break;
            }

            var slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = currentSelectedIndex;
        }
    }
}