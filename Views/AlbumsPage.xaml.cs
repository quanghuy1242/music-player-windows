using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;

namespace MPA.Views
{
    public sealed partial class AlbumsPage : Page
    {
        public ObservableCollection<Album> Albums { get; set; }

        public AlbumsPage()
        {
            this.DataContext = this;

            // Generate 500 albums to test virtualization and performance
            Albums = new ObservableCollection<Album>();
            var sampleUrls = new[]
            {
                "https://contents.quanghuy.dev/118CD291-17C4-4E0E-B51C-D8504A57E4D5_sk1.jpeg",
                "https://contents.quanghuy.dev/35F87834-A50F-40FB-9F76-E994D99D2656_sk1.jpeg",
                "https://contents.quanghuy.dev/60080A59-43AF-448E-99C1-85887045E5DC_sk1.jpeg",
                "https://contents.quanghuy.dev/73494CD3-B6D7-4931-8978-CD3E3C6EC7EF_sk1.jpeg",
                "https://contents.quanghuy.dev/79EEE411-BF3C-4F63-BD5E-39C673FFA737_sk1.jpeg",
            };

            for (int i = 0; i < 500; i++)
            {
                Albums.Add(new Album
                {
                    Title = $"Album {i + 1}",
                    Subtitle = $"Artist {(i % 20) + 1}",
                    ImageUrl = sampleUrls[i % sampleUrls.Length]
                });
            }

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

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Album album)
            {
                // TODO: Implement play album logic
                // For now, just show a message or navigate
            }
        }

        private void AlbumGrid_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // Use phased updates so expensive work (image decoding) happens later and
            // only for items that are actually coming into view. This follows MS guidance
            // for ListView/GridView performance and virtualization.
            var root = args.ItemContainer?.ContentTemplateRoot as FrameworkElement;
            if (root == null)
            {
                return;
            }

            var image = root.FindName("AlbumImage") as Image;
            var fallback = root.FindName("FallbackIcon") as Viewbox;

            // If the container is being recycled, clear the image to avoid showing previous content
            if (args.InRecycleQueue)
            {
                if (image != null)
                {
                    image.Source = null;
                }

                if (fallback != null)
                {
                    fallback.Visibility = Visibility.Visible;
                }

                return;
            }

            // Phase 0: quick setup - clear any previous source and show placeholder/fallback
            if (image != null)
            {
                image.Source = null;
                if (fallback != null)
                {
                    fallback.Visibility = Visibility.Visible;
                }

                // Register a callback so the framework will call us again to perform the heavier work
                // (creating BitmapImage and setting UriSource) only when the item is actually needed.
                args.RegisterUpdateCallback(new Windows.Foundation.TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs>(LoadImageCallback));
            }
        }

        private void LoadImageCallback(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var root = args.ItemContainer?.ContentTemplateRoot as FrameworkElement;
            if (root == null) return;

            var image = root.FindName("AlbumImage") as Image;
            var fallback = root.FindName("FallbackIcon") as Viewbox;

            if (image == null) return;

            var url = image.Tag as string;
            if (string.IsNullOrEmpty(url))
            {
                // No URL, keep fallback visible
                if (fallback != null) fallback.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                // Create BitmapImage and attach it to the Image control before setting UriSource
                // This follows MS guidance: connect the BitmapImage to the tree before setting UriSource
                // and use DecodePixelWidth/DecodePixelType to avoid decoding at full resolution.
                var bitmap = new BitmapImage()
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelWidth = 180
                };

                // Attach to the Image first to ensure the framework won't block UI when setting UriSource
                image.Source = bitmap;

                // Set UriSource after assigning Source to start asynchronous download/decoding
                bitmap.UriSource = new Uri(url);

                // Fallback will be hidden when ImageOpened fires
            }
            catch
            {
                // On any failure, ensure we show fallback
                if (fallback != null) fallback.Visibility = Visibility.Visible;
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