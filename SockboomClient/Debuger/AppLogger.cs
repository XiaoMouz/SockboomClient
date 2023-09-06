using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SockboomClient.Debuger
{
    /// <summary>
    /// Logger 日志
    /// </summary>
    public static class AppLogger
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }

        public static void LogWarn(string message)
        {
            _logger.Warn(message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            if (ex == null)
            {
                _logger.Error(message);
            }
            else
            {
                _logger.Error(message, ex);
            }
        }

        public static void LogDebug(string message)
        {
            _logger.Debug(message);
        }
    }
}
