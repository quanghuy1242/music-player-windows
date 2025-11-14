using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MPA.Views
{
    public sealed partial class AlbumPage : Page
    {
        public ObservableCollection<Album> Albums { get; set; }

        public AlbumPage()
        {
            this.DataContext = this;
            Albums = new ObservableCollection<Album>
            {
                new Album { Title = "Nữ Thần Mất Trăng (Mônangel)", Subtitle = "Bùi Lan Huong", ImageUrl = "https://contents.quanghuy.dev/118CD291-17C4-4E0E-B51C-D8504A57E4D5_sk1.jpeg" },
                new Album { Title = "The Human Era (Original Soundtrack)", Subtitle = "Epic Mountain", ImageUrl = "https://contents.quanghuy.dev/35F87834-A50F-40FB-9F76-E994D99D2656_sk1.jpeg" },
                new Album { Title = "Thiên Thần Sa Ngã", Subtitle = "Bùi Lan Huong", ImageUrl = "https://contents.quanghuy.dev/60080A59-43AF-448E-99C1-85887045E5DC_sk1.jpeg" },
                new Album { Title = "Lust for Life", Subtitle = "Lana Del Rey", ImageUrl = "https://contents.quanghuy.dev/73494CD3-B6D7-4931-8978-CD3E3C6EC7EF_sk1.jpeg" },
                new Album { Title = "Firewatch (Original Soundtrack)", Subtitle = "Chris Remo", ImageUrl = "https://contents.quanghuy.dev/79EEE411-BF3C-4F63-BD5E-39C673FFA737_sk1.jpeg" },
            };
            this.InitializeComponent();
        }

        private void Image_ImageOpened(object sender, RoutedEventArgs e)
        {
            // Hide fallback icon when image loads successfully
            if (sender is Image image)
            {
                var grid = VisualTreeHelper.GetParent(image) as Grid;
                if (grid != null)
                {
                    var fallbackIcon = grid.FindName("FallbackIcon") as Viewbox;
                    if (fallbackIcon != null)
                    {
                        fallbackIcon.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // Show fallback icon when image fails to load
            if (sender is Image image)
            {
                var grid = VisualTreeHelper.GetParent(image) as Grid;
                if (grid != null)
                {
                    var fallbackIcon = grid.FindName("FallbackIcon") as Viewbox;
                    if (fallbackIcon != null)
                    {
                        fallbackIcon.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }

    public class Album
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
    }
}