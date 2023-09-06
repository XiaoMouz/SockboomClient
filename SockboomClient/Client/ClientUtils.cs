using SockboomClient.Client.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace SockboomClient.Client
{
    internal static class ClientUtils
    {
        static string url = "https://api.sockboom.click/client";
        /// <summary>
        /// 返回完整 url 供HttpClient请求
        /// </summary>
        /// <param name="path">请求的接口</param>
        /// <returns>完整url</returns>
        internal static string GetUrl(GetPaths path)
        {
            switch (path)
            {
                case GetPaths.TRAFFIC: return url + "/traffic";
                case GetPaths.CHECKIN: return url + "/checkin";
                case GetPaths.SUBSCRIBE: return url + "/subscribe";
                case GetPaths.PAY: return url + "/pay";
                case GetPaths.CHECKPAY: return url + "/checkpay";
                case GetPaths.BUY: return url + "/buy";
                default: return url;
            }
        }

        internal static string GetUrl(PostPaths path)
        {
            switch (path)
            {
                case PostPaths.GETTOKEN: return url + "/getToken";
                default: return url;
            }
        }
    }
}
