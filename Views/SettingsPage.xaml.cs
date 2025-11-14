using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MPA.Views
{
    public sealed partial class SettingsPage : Page
    {
        private const string DarkModeKey = "DarkModeEnabled";

        public SettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the saved setting
            var localSettings = ApplicationData.Current.LocalSettings;
            bool isDarkMode = localSettings.Values.TryGetValue(DarkModeKey, out object value) && (bool)value;

            // Set the toggle
            DarkModeToggle.IsOn = isDarkMode;

            // Apply the theme
            ApplyTheme(isDarkMode);
        }

        private void DarkModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            bool isDarkMode = DarkModeToggle.IsOn;

            // Save the setting
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[DarkModeKey] = isDarkMode;

            // Apply the theme
            ApplyTheme(isDarkMode);
        }

        private void ApplyTheme(bool isDarkMode)
        {
            var theme = isDarkMode ? ElementTheme.Dark : ElementTheme.Light;

            // Find the ShellPage by traversing the visual tree
            DependencyObject parent = this.Parent;
            while (parent != null && !(parent is ShellPage))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is ShellPage shellPage)
            {
                shellPage.RequestedTheme = theme;
            }
        }
    }
}