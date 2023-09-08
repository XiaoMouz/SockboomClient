using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SockboomClient.Config;
using SockboomClient.Common;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;

namespace SockboomClient.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ClearLoginSettings();
            var app = (App)Application.Current;
            //app.BackToLoginWindow(); tpdo: function bug
            app.Exit();
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            // Get right now theme
            var currentTheme = Settings.Theme;
            switch (currentTheme)
            {
                case ApplicationTheme.Light:
                    themeMode.SelectedIndex = 0;
                    break;
                case ApplicationTheme.Dark:
                    themeMode.SelectedIndex = 1;
                    break;
                default:
                    themeMode.SelectedIndex = 2;
                    break;
            }
        }

        private void ThemeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;
            string selectedTag = (string)selectedItem.Tag;
            var app = (App)Application.Current;
            if (selectedTag == "Light")
            {
                UpdateTheme(ElementTheme.Light,sender);

            }
            else if (selectedTag == "Dark")
            {
                UpdateTheme(ElementTheme.Dark,sender);
            }
            else if (selectedTag == "Default")
            {
                UpdateTheme(ElementTheme.Default,sender);
            }
        }
        private void UpdateTheme(ElementTheme theme,object sender)
        {
            if(theme == ElementTheme.Default)
            {
                ((sender as ComboBox).XamlRoot.Content as Grid).RequestedTheme = Win32.GetUserSystemTheme() ? ElementTheme.Dark : ElementTheme.Light;
            }
            else
            {
                ((sender as ComboBox).XamlRoot.Content as Grid).RequestedTheme = theme;
            }
            switch (theme)
            {
                case ElementTheme.Light:Settings.Theme = ApplicationTheme.Light; break;
                case ElementTheme.Dark: Settings.Theme = ApplicationTheme.Dark; break;
                default: Settings.Theme = (ApplicationTheme)2; break;
            }
        }

    }
}
