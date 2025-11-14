using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using MPA.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Microsoft.UI.Windowing;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MPA
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        private ShellPage? _shellPage;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new Window();
            _window.SystemBackdrop = new MicaBackdrop();

            // Set minimum window size
            var appWindow = _window.AppWindow;
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 900;
                presenter.PreferredMinimumHeight = 600;
            }
            appWindow.Resize(new SizeInt32(900, 600));

            _shellPage = new ShellPage();
            _window.Content = _shellPage;

            // Customize title bar
            _window.ExtendsContentIntoTitleBar = true;
            //_window.SetTitleBar(_shellPage.TitleBarHost);

            // Restore window position and size
            RestoreWindowPosition();

            _window.Closed += OnWindowClosed;

            _window.Activate();
        }

        private void RestoreWindowPosition()
        {
            if (_window == null) return;

            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("WindowLeft", out var leftObj) &&
                localSettings.Values.TryGetValue("WindowTop", out var topObj) &&
                localSettings.Values.TryGetValue("WindowWidth", out var widthObj) &&
                localSettings.Values.TryGetValue("WindowHeight", out var heightObj))
            {
                int left = Convert.ToInt32(leftObj);
                int top = Convert.ToInt32(topObj);
                int width = Math.Max(900, Convert.ToInt32(widthObj));
                int height = Math.Max(900, Convert.ToInt32(heightObj));

                _window.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(left, top, width, height));
            }
        }

        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            if (_window == null) return;

            var localSettings = ApplicationData.Current.LocalSettings;
            var position = _window.AppWindow.Position;
            var size = _window.AppWindow.Size;

            localSettings.Values["WindowLeft"] = position.X;
            localSettings.Values["WindowTop"] = position.Y;
            localSettings.Values["WindowWidth"] = size.Width;
            localSettings.Values["WindowHeight"] = size.Height;
        }

        public void NavigateToAlbums()
        {
            _shellPage?.NavigateTo("Albums");
        }
    }
}

