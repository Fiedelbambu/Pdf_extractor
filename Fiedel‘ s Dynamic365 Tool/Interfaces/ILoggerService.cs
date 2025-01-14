using System;

namespace FiedelsDynamic365Tool.Interfaces
{
    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogError(string message, Exception? ex = null);
             
    }
}
