using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SockboomClient.Config;

namespace SockboomClient.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ClearLoginSettings();
            var app = (App)Application.Current;
            //app.BackToLoginWindow(); tpdo: function bug
            app.Exit();
        }
    }
}
