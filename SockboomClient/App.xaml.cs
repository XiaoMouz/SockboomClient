using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SockboomClient.Client;
using SockboomClient.Config;
using SockboomClient.Model;
using SockboomClient.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SockboomClient
{

    public partial class App : Application
    {
        private SharedViewModel _vm;
        private Window m_window;
        public App()
        {
            this.InitializeComponent();
            _vm = SharedViewModel.GetInstance();
            App.Current.RequestedTheme = Settings.Theme;
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            if (Settings.AutoLogin)
            {

                var Token = Settings.Token;
                try
                {
                    var Result = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string>
                    {
                        { "token", Token }
                    });
                    if (Result.Success)
                    {
                        var r = Result.Data;
                        r.Token = Token;
                        _vm.UserInfo = r;
                        m_window = new MainWindow();
                    }
                    else
                    {
                        m_window = new LoginWindow();

                    }
                }
                catch (Exception ex)
                {
                    m_window = new LoginWindow("发生错误", "自动登录失败:" + ex.Message);
                }
            }
            else
            {
                m_window = new LoginWindow();
            }
            m_window.Activate();
        }

        /// <summary>
        /// 返回登录窗口
        /// </summary>
        public void BackToLoginWindow()
        {
            m_window.Close();
            m_window = new LoginWindow();
            m_window.Activate();
        }
    }
}
