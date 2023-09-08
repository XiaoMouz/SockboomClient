using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
            TestText.Text = _vm.UserInfo.Money.ToString();
        }
    }
}
