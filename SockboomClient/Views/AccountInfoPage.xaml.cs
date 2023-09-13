using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SockboomClient.Debuger;
using SockboomClient.Model;
using SockboomClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SockboomClient.Views
{
    /// <summary>
    ///用户信息主体页面
    /// </summary>
    public sealed partial class AccountInfoPage : Page
    {
        private SharedViewModel _vm;
        public AccountInfoPage()
        {
            this.InitializeComponent();
            _vm = SharedViewModel.GetInstance();
            DataContext = _vm;
        }

        private void TotalTrafficBar_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置流量占比条
            double infoPercent = (double)_vm.UserInfo.UsedTotal / (double)_vm.UserInfo.Total;
            infoPercent = infoPercent * 100;
            if (infoPercent >= 100) { infoPercent = 100; }
            TotalTrafficBar.Value = (int)infoPercent;
            if (infoPercent > 80 && infoPercent != 100) TotalTrafficBar.ShowPaused = true;
            if (infoPercent == 100) TotalTrafficBar.ShowError = true;

            if (_vm.CheckinModel.CheckinMessage==null|| _vm.CheckinModel.CheckinMessage.Equals(""))
            {
                CheckinButtonText.Text = "签到";
                CheckinButton.IsEnabled = true;
            }
                
        }

        private void SSRLinkButton_Click(object sender, RoutedEventArgs args)
        {
            var package = new DataPackage();
            package.SetText(_vm.UserInfo.SSRSub);
            Clipboard.SetContent(package);
        }

        private void ClashLinkButton_Click(object sender, RoutedEventArgs args)
        {
            var package = new DataPackage();
            package.SetText(_vm.UserInfo.ClashSub);
            Clipboard.SetContent(package);
        }

        private void CheckinButton_Click(object sender, RoutedEventArgs args)
        {
            CheckinButton.IsEnabled = false;
            CheckinButtonProgressRing.Visibility = Visibility.Visible;
            _vm.CheckinRequest();
            CheckinButtonProgressRing.Visibility = Visibility.Collapsed;
        }
    }
}
