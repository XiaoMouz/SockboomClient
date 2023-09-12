using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Model
{
    public class CheckinModel
    {
        /// <summary>
        /// 签到活动是否可用， true 为可用 false 时不可用
        /// </summary>
        public bool CheckinEnable { set; get; }
        /// <summary>
        /// 签到进度 WIP，无用
        /// </summary>
        public int CheckinStatus { set; get; }
        /// <summary>
        /// 签到消息信息
        /// </summary>
        public string CheckinMessage { set; get; }
        /// <summary>
        /// 签到后获取的流量
        /// </summary>
        public long CheckinTraffic { set; get; }
        /// <summary>
        /// 签到后获取的流量转为 String 流量单位表达
        /// </summary>
        public string CheckinTrafficByString
        {
            get
            {
                return GetStringValue(CheckinTraffic);
            }
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
