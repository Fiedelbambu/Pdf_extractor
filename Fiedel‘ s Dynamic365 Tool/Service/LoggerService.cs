using System;
using FiedelsDynamic365Tool.Interfaces;
using log4net;

namespace FiedelsDynamic365Tool.Service
{
    public class LoggerService : ILoggerService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoggerService));

        public void LogInfo(string message)
        {
            log.Info(message);
        }

        public void LogWarn(string message)
        {
            log.Warn(message);
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                log.Error(message, ex);
            }
            else
            {
                log.Error(message);
            }
        }
    }
}
