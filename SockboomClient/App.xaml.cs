using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
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
using SockboomClient.Debuger;

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
            string secret = Environment.GetEnvironmentVariable("APP_CENTER_SECRET");
            if (secret != null)
            {
                AppCenter.Start(secret,
                  typeof(Analytics), typeof(Crashes));
                AppLogger.LogInfo("Enabled App center analytics");
            }
            else
            {
                AppLogger.LogWarn("Disabled App center analytics");
            }

            if (Settings.AutoLogin)
            {
                AppLogger.LogInfo("AutoLogin is enable, Start login progress");
                var Token = Settings.Token;
                m_window = new LoginWindow(Token);


                //var Result = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string>
                //{
                //    { "token", Token }
                //});
                //if (Result.Success)
                //{
                //    var r = Result.Data;
                //    r.Token = Token;
                //    _vm.UserInfo = r;
                //    m_window = new MainWindow();
                //}
                //else
                //{
                //    m_window = new LoginWindow();

                //}
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
