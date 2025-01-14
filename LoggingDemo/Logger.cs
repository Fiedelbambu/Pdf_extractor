using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LoggingDemo
{
    public class Logger
    {
        private static readonly string DefaultLogFilePath = "application.log";
        private static readonly string DefaultFeatureLogFilePath = "features.log";

        private readonly string _logFilePath;
        private readonly string _featureLogFilePath;

        public Logger(string logFilePath = null, string featureLogFilePath = null)
        {
            _logFilePath = logFilePath ?? DefaultLogFilePath;
            _featureLogFilePath = featureLogFilePath ?? DefaultFeatureLogFilePath;
        }

        public void Log(string message)
        {
            string logMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}: {message}";
            File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
        }

        public void LogFeature(string feature)
        {
            string featureLogMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}: FEATURE: {feature}";
            File.AppendAllText(_featureLogFilePath, featureLogMessage + Environment.NewLine);
        }
    }
}
