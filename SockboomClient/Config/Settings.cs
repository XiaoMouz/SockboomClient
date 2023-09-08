using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SockboomClient.Config
{
    public static class Settings
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public static bool AutoLogin
        {
            get
            {
                if (localSettings.Values.TryGetValue("AutoLogin", out object AutoLoginInfo))
                {
                    return AutoLoginInfo.Equals("true");
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    ApplicationData.Current.LocalSettings.Values["AutoLogin"] = "true";
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["AutoLogin"] = "false";
                }
            }
        }
        public static string Token
        {
            get
            {
                if (localSettings.Values.TryGetValue("Token", out object Token))
                {
                    return Token.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ApplicationData.Current.LocalSettings.Values["Token"] = value;
            }
        }

        public static void ClearLoginSettings()
        {
            
            ApplicationData.Current.LocalSettings.Values["AutoLogin"] = "false";
            ApplicationData.Current.LocalSettings.Values["Token"] = null;
            ApplicationData.Current.LocalSettings.Values.Remove("AutoLogin");
            ApplicationData.Current.LocalSettings.Values.Remove("Token");
        }
    }
}
