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

        private CheckinModel _checkinModel;
        public CheckinModel CheckinModel
        {
            get { return _checkinModel; }
            set
            {
                if (_checkinModel != value)
                {
                    _checkinModel = value;
                    OnPropertyChanged(nameof(CheckinModel));
                }
            }
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
                    OnPropertyChanged(nameof(UserInfo));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 更新用户信息
        /// </summary>
        public async Task<bool> RequestUpdateUserInfo() 
        {
            try
            {
                if (UserInfo == null)
                {
                    return false;
                }
                if (UserInfo.Token == null)
                {
                    return false;
                }
                var Result = await UserInfo.UpdateUserInfo();
                var ssrLink = await UserInfo.UpdateUserSub();
                if (Result.Success)
                {
                    var cacheToken = UserInfo.Token;
                    UserInfo = Result.Data;
                    UserInfo.Token = cacheToken;
                    OnPropertyChanged(UserInfo.GetType().Name);
                    OnPropertyChanged(GetType().Name);
                    return true;
                }
                else
                {
                    return false;
                }
            }catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 进行用户签到并更新状态
        /// </summary>
        public async void CheckinRequest()
        {
            if(CheckinModel == null)
            {
                _checkinModel = new CheckinModel();
                CheckinModel.CheckinMessage = "签到";
                CheckinModel.CheckinEnable = true;
                OnPropertyChanged(nameof(CheckinModel));
            }
            if(UserInfo.Token == null || UserInfo.Token.Equals(""))
            {
                CheckinModel.CheckinStatus = 418;
                CheckinModel.CheckinEnable = false;
                CheckinModel.CheckinMessage = "无法签到,缺少 Token";
                OnPropertyChanged(nameof(CheckinModel));
                return;
            }
            CheckinModel.CheckinEnable = false;
            CheckinModel.CheckinMessage = "签到中";
            OnPropertyChanged(nameof(CheckinModel));
            var checkinResult = await ApiClient.GetRequest<string>(Client.Apis.GetPaths.CHECKIN, new Dictionary<string, string> { { "token", UserInfo.Token } });
            if (checkinResult.Success)
            {
                CheckinModel.CheckinTraffic = checkinResult.TrafficByLong;
                CheckinModel.CheckinMessage = "签到成功,获得了 " + CheckinModel.CheckinTrafficByString + "流量";
            }else if(checkinResult.Code == 0)
            {
                CheckinModel.CheckinMessage = "你今天已经签过到了";
            }
            else
            {
                CheckinModel.CheckinMessage = "签到失败";
                CheckinModel.CheckinEnable = true;
            }
            OnPropertyChanged(nameof(CheckinModel));
        }
    }
}
