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
            DataContext = _vm.UserInfo;
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
        }

        private void SubscribeStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            SSRSubText.Text = _vm.UserInfo.SSRSub;
            ClashSubText.Text = _vm.UserInfo.ClashSub;
        }
    }
}
