using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using SockboomClient.Client;
using SockboomClient.Compose;
using SockboomClient.Model;
using SockboomClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx.Messaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SockboomClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShopPage : Page
    {
        SharedViewModel _vm;
        List<Commodity> commodities;
        public ShopPage()
        {
            this.InitializeComponent();
            _vm = SharedViewModel.GetInstance();
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string json;
                // 读取项目内的 CommodityInfo.json
                using (Stream stream = assembly.GetManifestResourceStream("SockboomClient.CommodityInfo.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
                commodities = JsonConvert.DeserializeObject<List<Commodity>>(json);
            }catch (Exception)
            {
                //nothing todo
            }
        }

        private void RootGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if(_vm.UserInfo._level > 0)
            {
                VIPTipsInfo.IsOpen = true;
                VIPBuyButton.IsEnabled = false;
            }

            // 加载套餐
            if (commodities != null)
            {
                foreach (Commodity commodity in commodities)
                {
                    if (commodity.Id == 2)
                        continue;
                    commoditySVPanel.Children.Add(new CommodityCard(commodity, _vm.UserInfo.Token));
                }
            }
            else { 
                var info = new InfoBar();
                info.IsOpen = true;
                info.Title = "出错了";
                info.Message = "获取套餐列表失败";
                info.Severity = InfoBarSeverity.Error;
                commoditySVPanel.Children.Add(info);
            }
            
        }

        /// <summary>
        /// 购买 VIP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VIPBuyButton_Click(object sender, RoutedEventArgs e)
        {
            VIPBuyButton.IsEnabled = false;

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
                var s = await SendVIPBuyRequest(content);
                if (s.Success)
                {
                    await _vm.RequestUpdateUserInfo();
                    dialog.Hide();
                    ShowDialog("成功", "成功购买了 VIP");
                }
                else
                {
                    dialog.Hide();
                    ShowDialog("失败了", "原因:" + s.Message);
                }
                
            };
            dialog.Content = new Dialog(content,"如没有优惠码点击继续即可");
            await dialog.ShowAsync();
        }
        private async Task<HttpResult<string>> SendVIPBuyRequest(object sender)
        {
            TextBox code = sender as TextBox;
            if(code.Text != null || code.Text.Length != 0) {
                var result = await ApiClient.GetRequest<string>(Client.Apis.GetPaths.BUY, new Dictionary<string, string>
                {
                    { "token", _vm.UserInfo.Token },
                    {"code", code.Text },
                    {"shop", "2" },
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
