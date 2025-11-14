using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MPA.ViewModels;

namespace MPA.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellPage()
        {
            ViewModel = new PlaybackViewModel(DispatcherQueue);
            InitializeComponent();
            DataContext = ViewModel;
            Loaded += OnLoaded;
        }

        public PlaybackViewModel ViewModel { get; }

        public UIElement TitleBarHost => TitleBarDragRegion;

        public void NavigateTo(string targetTag)
        {
            if (string.IsNullOrEmpty(targetTag))
            {
                return;
            }

            var targetItem = RootNavigationView.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => string.Equals(item.Tag as string, targetTag, StringComparison.Ordinal));

            if (targetItem != null && !ReferenceEquals(RootNavigationView.SelectedItem, targetItem))
            {
                RootNavigationView.SelectedItem = targetItem;
            }

            NavigateFrame(targetTag);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigateTo("Home");
        }

        private void OnNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer is NavigationViewItem item && item.Tag is string tag)
            {
                NavigateFrame(tag);
            }
        }

        private void NavigateFrame(string tag)
        {
            var targetPage = tag switch
            {
                "Albums" => typeof(AlbumPage),
                "Settings" => typeof(SettingsPage),
                _ => typeof(HomePage)
            };

            if (ContentFrame.CurrentSourcePageType != targetPage)
            {
                ContentFrame.Navigate(targetPage);
            }
        }
    }
}
