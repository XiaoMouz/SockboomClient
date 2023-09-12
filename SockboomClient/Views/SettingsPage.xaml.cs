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
using SockboomClient.ViewModel;
using SockboomClient.Compose;
using System.Threading;

namespace SockboomClient.Views
{
    public sealed partial class SettingsPage : Page
    {
        SharedViewModel _vm;
        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;
            _vm = SharedViewModel.GetInstance();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.ClearLoginSettings();
            var app = (App)Application.Current;
            //app.BackToLoginWindow(); tpdo: function bug
            app.Exit();
        }

        /// <summary>
        /// 设置页初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            // 主题设置
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

            // 自动登录检测
            var autoCheckin = Settings.AutoCheckin;
            if (autoCheckin)
            {
                AutoCheckinSwitch.IsOn = true;
            }
            else
            {
                AutoCheckinSwitch.IsOn = false;
            }
        }

        /// <summary>
        /// 自动登录切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoCheckinSwitch_Toggled(object sender,RoutedEventArgs e)
        {
            if (AutoCheckinSwitch.IsOn)
            {
                Settings.AutoCheckin = true;
            }
            else
            {
                Settings.AutoCheckin = false;
            }
        }

        /// <summary>
        /// 主题修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        private async void ShowDialog(string title, string message)
        {
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = title;
            dialog.PrimaryButtonText = "好";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new Dialog(message);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SyncButton_OnClick(object sender, RoutedEventArgs e)
        {
            SyncButton.IsEnabled = false;
            SyncProgressRing.Visibility = Visibility.Visible;
            SyncButtonText.Text = "同步中";
            if (await _vm.RequestUpdateUserInfo())
            {
                ShowDialog("同步成功", "您的账户信息已同步");
            }
            else
            {
                ShowDialog("同步失败", "请检查网络或测试与服务器连通性");
            }
            
            SyncButton.IsEnabled = true;
            SyncProgressRing.Visibility = Visibility.Collapsed;
            SyncButtonText.Text = "同步";
        }
    }
}
