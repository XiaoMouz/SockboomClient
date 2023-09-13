using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SockboomClient.Client;
using SockboomClient.Compose;
using SockboomClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;
using WinUIEx.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SockboomClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RechargePage : Page
    {
        SharedViewModel _vm;

        string orderNum = String.Empty;
        public RechargePage()
        {
            this.InitializeComponent();
            _vm = SharedViewModel.GetInstance();
            this.DataContext = _vm;

        }


        public async void RootGrid_Loaded(Object sender, RoutedEventArgs e)
        {
            // 测试支付接口状态
            RefreshButton.IsEnabled = false;
            var Result = await ApiClient.GetRequest<string>(Client.Apis.GetPaths.PAY, new Dictionary<string, string> { { "token", _vm.UserInfo.Token } });
            if (Result.Code == 500)
            {
                PaymentButton.IsEnabled = false;
                RefreshButton.Visibility = Visibility.Visible;
                InfoBarTip.Message = $"支付接口目前暂不可用: {Result.Code} - {Result.Message}";
                InfoBarTip.IsOpen = true;
                RefreshButton.IsEnabled = true;
            }
            else
            {
                PaymentButton.IsEnabled = true;
                RefreshButton.Visibility = Visibility.Collapsed;
                InfoBarTip.IsOpen = false;
            }
        }
        public async void PaymentButton_Click(Object sender, RoutedEventArgs e)
        {
            var paymentplatform = ((TextBlock)PaymentPlatformRadiosButton.SelectedItem).Tag.ToString();
            var price = PriceInput.Value;
            var Result = await ApiClient.GetRequest<string>(Client.Apis.GetPaths.PAY, new Dictionary<string, string> { { "token", _vm.UserInfo.Token },{ "type", paymentplatform },{ "price",price.ToString() } });

            if (Result.Code == 0|| Result.Success)
            {
                new BrowserWindow(Result.Data);
                this.orderNum = Result.Pid;
            }

            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "支付请求已发送";
            dialog.PrimaryButtonText = "已支付";
            dialog.SecondaryButtonText = "取消支付";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new Dialog("等待用户支付");
            await dialog.ShowAsync();

            // todo: 验证用户支付情况, 更新用户信息
        }
    }

    
}
