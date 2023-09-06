using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using SockboomClient.Client;
using SockboomClient.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SockboomClient
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 尝试获取自动登录信息
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue("AutoLogin", out object AutoLoginInfo))
            {
                // convert to dictionary
                var AutoLogin = AutoLoginInfo  as string;
                if (AutoLogin.Equals("true"))
                {
                    localSettings.Values.TryGetValue("Token", out object AutoLoginToken);
                    var Token =  AutoLoginToken as string;
                    var Result = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string>
                    {
                        { "token", Token }
                    });
                    if (Result.Success)
                    {
                        m_window = new MainWindow(Result.Data);
                    }
                    else
                    {
                        m_window = new LoginWindow();
                    }
                }
                else
                {
                    m_window = new LoginWindow();
                }
            }
            else
            {
                m_window = new LoginWindow();
            }
            m_window.Activate();
        }

        public Window m_window;
    }
}
