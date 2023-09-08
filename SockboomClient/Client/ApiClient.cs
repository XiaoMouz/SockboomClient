using Newtonsoft.Json;
using SockboomClient.Client.Apis;
using SockboomClient.Debuger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace SockboomClient.Client
{
    public static class ApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        /// <summary>
        /// Get 请求方法
        /// </summary>
        /// <typeparam name="T">返回结果数据类</typeparam>
        /// <param name="path">GET请求枚举地址</param>
        /// <param name="queryParams">自定义请求参数</param>
        /// <returns>Task</returns>
        public static async Task<HttpResult<T>> GetRequest<T>(
            GetPaths path,
            Dictionary<string, string> queryParams
            )
        {
            try
            {
                string url = ClientUtils.GetUrl(path); // 获取对应接口地址
                if (queryParams != null && queryParams.Count > 0)
                {
                    var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    url += "?" + queryString;
                    AppLogger.LogDebug($"Request[type:GET] URL: {queryString}");
                }


                var response = await _httpClient.GetAsync(url);

                return await AnalysisResponse<T>(response);
            }catch(Exception ex)
            {
                AppLogger.LogError(ex.Message);
                return await AnalysisResponse<T>(null,ex);
            }
            
        }

        /// <summary>
        /// Post 请求
        /// </summary>
        /// <typeparam name="T">返回的数据类</typeparam>
        /// <param name="path">Post 请求路径</param>
        /// <param name="data">请求的数据</param>
        /// <returns>Task</returns>
        public static async Task<HttpResult<T>> PostRequest<T>(PostPaths path, Dictionary<string,string> data)
        {
            try
            {
                var content = new FormUrlEncodedContent(data);
                string url = ClientUtils.GetUrl(path); // 获取对应接口地址
                var response = await _httpClient.PostAsync(url, content); // 发送请求
                return await AnalysisResponse<T>(response);
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex.Message);
                return await AnalysisResponse<T>(null,ex);
            }

            
            
        }
#nullable enable
        /// <summary>
        /// 处理请求后返回的响应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private async static Task<HttpResult<T>> AnalysisResponse<T> (HttpResponseMessage? response, Exception? ex = null)
        {
            HttpResult<T> r = new HttpResult<T>();
            if (ex != null)
            {
                r.Code = 0;
                r.Message = ex.Message;
                return r;
            }

            // 服务端 HTTP CODE 检测
            if (response?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                AppLogger.LogDebug(responseContent);
                try
                {
                    var responseData = JsonConvert.DeserializeObject<HttpResult<T>>(responseContent); // 反序列化
                    if (responseData != null && responseData.Data != null)
                    {
                        AppLogger.LogDebug($"Response: {responseData.Data}");
                    }
                    else
                    {
                        AppLogger.LogDebug($"Response: null");
                    }
                    if(responseData == null)
                    {
                        throw new Exception();
                    }
                    return responseData;
                }
                catch (Exception e)
                {
                    r.Code = 500;
                    r.Message = "客户端出现错误:" + e.Message;
                    return r;
                }
            }
            r.Code = 500;
            r.Message = "响应出现错误。请重试, 服务端状态码:" + response?.StatusCode;
            return r;

        }
    }
#nullable disable
}
