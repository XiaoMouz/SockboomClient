using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SockboomClient.Client;
using SockboomClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SockboomClient.Compose
{
    public sealed partial class CommodityCard : UserControl
    {
        private string token;
        private Commodity Commodity;
        public CommodityCard(Commodity commodity,string token)
        {
            this.InitializeComponent();
            this.DataContext = commodity;
            this.Commodity = commodity;
            this.token = token;
        }

        /// <summary>
        /// 购买 VIP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            BuyButton.IsEnabled = false;

            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "输入您的优惠码";
            dialog.PrimaryButtonText = "购买";

            dialog.SecondaryButtonText = "取消";
            dialog.DefaultButton = ContentDialogButton.Primary;
            var content = new TextBox();
            content.Name = "DiscountCodeInput";
            content.Margin = new Thickness(0, 5, 0, 0);
            content.Width = 200;
            content.PlaceholderText = "优惠码";
            content.HorizontalAlignment = HorizontalAlignment.Left;
            dialog.PrimaryButtonClick += async delegate
            {
                dialog.IsPrimaryButtonEnabled = false;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.PrimaryButtonText = "购买中";
                var s = await SendBuyRequest(content);
                if (s.Success)
                {
                    dialog.Hide();
                    ShowDialog("成功", "成功购买了 " + Commodity.Name);
                }
                else
                {
                    dialog.Hide();
                    ShowDialog("失败了", "原因:" + s.Message);
                    BuyButton.IsEnabled = true;
                }

            };
            dialog.Content = new Dialog(content, "如没有优惠码点击继续即可");
            await dialog.ShowAsync();
        }
        private async Task<HttpResult<string>> SendBuyRequest(object sender)
        {
            TextBox code = sender as TextBox;
            if (code.Text != null || code.Text.Length != 0)
            {
                var result = await ApiClient.GetRequest<string>(Client.Apis.GetPaths.BUY, new Dictionary<string, string>
                {
                    { "token", this.token },
                    {"code", code.Text },
                    {"shop", Commodity.Id.ToString() },
                    {"autorenew", "false" }
                });
                return result;
            }
            var r = new HttpResult<string>();
            r.Code = 418;
            r.Message = "客户端失败了";
            return r;

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
    }
}
