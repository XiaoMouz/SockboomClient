using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Client.Apis
{
    /// <summary>
    /// Get 请求路径
    /// </summary>
    public enum GetPaths
    {
        /// <summary>
        /// 通过 Token 获取用户信息 (流量等信息)
        /// </summary>
        [Description("用户信息")]
        TRAFFIC,
        /// <summary>
        /// 通过 Token 为用户签到
        /// </summary>
        [Description("用户签到")]
        CHECKIN,
        /// <summary>
        /// 通过 Token 获取用户订阅链接
        /// </summary>
        [Description("用户订阅")]
        SUBSCRIBE,
        /// <summary>
        /// 通过 Token 发起充值请求, 获取订单号
        /// </summary>
        [Description("用户充值")]
        PAY,
        /// <summary>
        /// 验证订单号是否成功充值
        /// </summary>
        [Description("订单检查")]
        CHECKPAY,
        /// <summary>
        /// 通过用户 Token 购买套餐
        /// </summary>
        [Description("套餐购买")]
        BUY
    }
}
