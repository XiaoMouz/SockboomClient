﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Model
{
    public class UserInfo
    {
        /// <summary>
        /// 用户 Token
        /// </summary>
        public string Token
        {
            get
            {
                return Token;
            }
            set
            {
                if(Token == null || Token == " ")
                {
                    Token = value;
                }
            }
        }

        /// <summary>
        /// 用户等级
        /// 0: 普通用户; 1: VIP; 2: Staff
        /// </summary>
        [JsonProperty("class")]
        public int Level;

        /// <summary>
        /// 用户当日使用流量 (单位: bit)
        /// </summary>
        [JsonProperty("used_today")]
        public int UsedToday;

        /// <summary>
        /// 用户共使用流量 (单位: bit)
        /// </summary>
        [JsonProperty("used_total")]
        public int UsedTotal;

        /// <summary>
        /// 用户剩余流量 (单位: bit)
        /// </summary>
        [JsonProperty("unused")]
        public int Unused;

        /// <summary>
        /// 用户流量有效期 (单位: 天)
        /// </summary>
        [JsonProperty("days")]
        public int Days;

        /// <summary>
        /// 用户账户余额
        /// </summary>
        [JsonProperty("money")]
        public int Money;
    }
}