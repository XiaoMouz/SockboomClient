using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Client.Apis
{
    /// <summary>
    /// Post 请求路径
    /// </summary>
    public enum PostPaths
    {
        /// <summary>
        /// 通过邮箱+密码获取用户 Token
        /// </summary>
        [Description("获取用户 Token")]
        GETTOKEN
    }
}
