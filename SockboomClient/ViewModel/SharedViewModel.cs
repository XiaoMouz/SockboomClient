using SockboomClient.Client;
using SockboomClient.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.ViewModel
{
    public class SharedViewModel : INotifyPropertyChanged
    {
        private SharedViewModel() { }

        private static SharedViewModel _instance;

        public static SharedViewModel GetInstance()
        {
            if (_instance == null)
                _instance = new SharedViewModel();

            return _instance;
        }

        private UserInfo _userInfo;
        public UserInfo UserInfo
        {
            get { return _userInfo; }
            set
            {
                if (_userInfo != value)
                {
                    _userInfo = value;
                    OnPropertyChanged(nameof(SharedViewModel));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 请求更新用户信息
        /// </summary>
        public async void RequestUpdateUserInfo()
        {
            if(UserInfo == null)
            {
                return;
            }
            if(UserInfo.Token == null)
            {
                return;
            }
            var Result = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string>
                    {
                        { "token", UserInfo.Token }
                    });
            if (Result.Success)
            {
                UserInfo = Result.Data;
            }
        }
    }
}
