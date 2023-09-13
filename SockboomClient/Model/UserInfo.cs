  using Newtonsoft.Json;
using SockboomClient.Client;
using SockboomClient.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Model
{
    /// <summary>
    /// Sockboom 用户信息
    /// </summary>
    public class UserInfo
    {
        private string token;
        /// <summary>
        /// 用户 Token
        /// </summary>
        public string Token
        {
            get
            {
                return token;
            }
            set {
                Settings.Token = value;
                token = value;
            }
        }

        /// <summary>
        /// 用户等级
        /// 0: 普通用户; 1: VIP; 2: Staff
        /// </summary>
        [JsonProperty("class")]
        public int _level
        {
            private set;
            get;
        }
        public string Level
        {
            get
            {
                switch(_level)
                {
                    case 0: return "普通";
                    case 1: return "VIP";
                    case 2: return "Staff";
                    default: return "未获取";
                }
            }
        }

        /// <summary>
        /// 用户当日使用流量 (单位: bit)
        /// </summary>
        [JsonProperty("used_today")]
        public long UsedToday;
        public string UsedTodayByString
        {
            get
            {
                return GetStringValue(UsedToday);
            }
        }

        /// <summary>
        /// 用户共使用流量 (单位: bit)
        /// </summary>
        [JsonProperty("used_total")]
        public long UsedTotal;
        public string UsedTotalByString
        {
            get
            {
                return GetStringValue(UsedTotal);
            }
        }

        /// <summary>
        /// 用户剩余流量 (单位: bit)
        /// </summary>
        [JsonProperty("unused")]
        public long Unused;
        public string UnusedByString
        {
            get
            {
               return GetStringValue(Unused);
            }
        }

        public long Total
        {
            get
            {
                return UsedTotal + Unused;
            }
        }
        public string TotalByString
        {
            get
            {
                return GetStringValue(Total);
            }
        } 

        /// <summary>
        /// 用户流量有效期 (单位: 天)
        /// </summary>
        [JsonProperty("days")]
        public int Days { get; set; }

        /// <summary>
        /// 用户账户余额
        /// </summary>
        [JsonProperty("money")]
        public double Money { get; set; }

        /// <summary>
        /// SSR 订阅链接
        /// </summary>
        public string SSRSub { get; set; }

        /// <summary>
        /// Clash 订阅链接
        /// </summary>
        public string ClashSub { get; set; }

        /// <summary>
        /// 获取新的用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResult<UserInfo>> UpdateUserInfo()
        {
            var info = await ApiClient.GetRequest<UserInfo>(Client.Apis.GetPaths.TRAFFIC, new Dictionary<string, string>
            {
                { "token",token }
            });
            return info;
        }

        /// <summary>
        /// 获取新的用户订阅
        /// </summary>
        /// <returns></returns>
        public async Task<string> UpdateUserSub()
        {
            var info = await ApiClient.GetRequest<string>(Client.Apis.GetPaths.SUBSCRIBE, new Dictionary<string, string>
            {
                { "token",token }
            });
            if (info.Success)
            {
                this.SSRSub = info.Subscribe;
                this.ClashSub = (@"https://sub-2.sockboom.pro/sub?target=clash&new_name=true&url=" + info.Subscribe + @"&filename=Sockboom&udp=true&config=https%3A%2F%2Fconfig.sockboom.me%2Fsubconfig.ini");
                return info.Subscribe;
            }
            return null;
        }
        private string GetStringValue(long value)
        {
            // 计算 Unused 从 1024 Bit 至 MB，若大于 1024 MB 则设为 GB, 若大于 1024 GB 则设为 TB
            if (value == 0) return "0M";
            double megabytes = value / (1024 * 1024);
            if (megabytes > 1024)
            {
                double gigabytes = megabytes / 1024;
                if (gigabytes > 1024)
                {
                    double terabytes = gigabytes / 1024;
                    terabytes = Math.Round(terabytes, 2);
                    return $"{terabytes} TB";
                }
                else
                {
                    gigabytes = Math.Round(gigabytes, 2);
                    return $"{gigabytes} GB";
                }
            }
            else
            {
                megabytes = Math.Round(megabytes, 2);
                return $"{megabytes} MB";
            }
        }
    }
}
