using DPSinfra.Logger;
using Newtonsoft.Json;

namespace SampleCodeAPI.Classes
{
    public class LoggerHelper
    {
        private readonly ILogger _logger;

        public LoggerHelper(ILogger logger)
        {
            _logger = logger;
        }

        void writeGeneralLog(string name, string mess, object data, LogLevel level)
        {
            var log = new GeneralLog()
            {
                name = name,
                message = mess,
                data = JsonConvert.SerializeObject(data),
            };
            logLevel(log, level);
        }

        public void writeActivityLog(string username, string category, string action, object data, LogLevel level)
        {
            var log = new ActivityLog()
            {
                username = username,
                category = category,
                action = action,
                data = JsonConvert.SerializeObject(data),
            };
            logLevel(log, level);
        }

        /// <summary>
        /// Write log activity: error
        /// </summary>
        /// <param name="username"></param>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public void LogError(string username, string category, string action, object data)
        {
            writeActivityLog(username, category, action, data, LogLevel.Error);
        }

        public void LogError(string username, string category, string action, Exception ex, object o = null)
        {
            if (ex == null)
                return;
            string data = ex.Message;
            data += "Detail:" + ex.StackTrace;
            if (o != null)
                data += "Data: " + JsonConvert.SerializeObject(o);
            writeActivityLog(username, category, action, data, LogLevel.Error);
        }

        /// <summary>
        /// Write log general: error
        /// </summary>
        /// <param name="username"></param>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public void LogError(string name, string mess, object data)
        {
            writeGeneralLog(name, mess, data, LogLevel.Error);
        }

        /// <summary>
        /// Write log general: warning
        /// </summary>
        /// <param name="username"></param>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public void LogWarn(string name, string mess, object data)
        {
            writeGeneralLog(name, mess, data, LogLevel.Warning);
        }

        /// <summary>
        /// Write log activity: infomation
        /// </summary>
        /// <param name="username"></param>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public void LogInfo(string username, string category, string action, object data)
        {
            writeActivityLog(username, category, action, data, LogLevel.Information);
        }

        /// <summary>
        /// Write log general: infomation
        /// </summary>
        /// <param name="username"></param>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <param name="data"></param>
        public void LogInfo(string name, string mess, object data)
        {
            writeGeneralLog(name, mess, data, LogLevel.Information);
        }

        private void logLevel(object log, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(JsonConvert.SerializeObject(log));
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(JsonConvert.SerializeObject(log));
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(JsonConvert.SerializeObject(log));
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(JsonConvert.SerializeObject(log));
                    break;
                case LogLevel.Error:
                    _logger.LogError(JsonConvert.SerializeObject(log));
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(JsonConvert.SerializeObject(log));
                    break;
            }
        }
    }
}
