using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Client
{
    /// <summary>
    /// Http Response 类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class HttpResult<T>
    {
        [JsonProperty("success")]
        public int Code { get; set; }

        /// <summary>
        /// 请求返回的消息
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; }

        #region 特殊请求返回的字段
        /// <summary>
        /// 通过登录接口请求返回的 TOKEN
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// 通过签到接口请求返回的本次签到获取的流量
        /// </summary>
        [JsonProperty("traffic")]
        public string Traffic { get; set;}

        public long TrafficByLong
        {
            get
            {
                try
                {
                    return Convert.ToInt64(Traffic);
                }catch(Exception)
                {
                    return 0;
                }
                
            }
        }

        /// <summary>
        /// 通过订阅接口获取的订阅链接
        /// </summary>
        [JsonProperty("sub")]
        public string Subscribe { get; set; }

        /// <summary>
        /// 通过充值接口获取的订单号
        /// </summary>
        [JsonProperty("pid")]
        public string Pid { get; set; }
        #endregion

        /// <summary>
        /// 异常传递
        /// </summary>
        public Exception Error { get; set; }

        public T Data { get; set; }

        public override string ToString()
        {
            if (Message == null)
                Message = string.Empty;
            if (Data == null)
                return @"{ code: " + Code + ",message: " + Message + ",data:null }";
            return @"{ code: " + Code + ",message: " + Message + ",data:{ " + Data.ToString() + " } }";
        }

        /// <summary>
        /// 本次请求服务器是否成功处理用户需求 （并非是否成功响应请求！）
        /// </summary>
        [JsonIgnore]
        public bool Success
        {
            get
            {
                if (Code == 1)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
