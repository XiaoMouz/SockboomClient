using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SockboomClient.Model
{
    /// <summary>
    /// 套餐信息
    /// </summary>
    public class Commodity
    {
        /// <summary>
        /// 套餐 ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 套餐流量 1024bit 计算制
        /// </summary>
        public long Traffic { get; set; }
        [JsonIgnore]
        public string TrafficByString
        {
            get
            {
                return GetStringValue(Traffic);
            }
        }   
        /// <summary>
        /// 带宽上限 单位 Mbps
        /// </summary>
        public int BandwidthLimit { get; set; }
        [JsonIgnore]
        public string BandwidthLimitByString
        {
            get
            {
                if(BandwidthLimit == 0)
                {
                    return "无上限";
                }
                return BandwidthLimit.ToString() + " Mbps";
            }
        }
        /// <summary>
        /// 套餐时长 单位 天
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 套餐价格 单位 元
        /// </summary>
        public double Price { get; set; }

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
