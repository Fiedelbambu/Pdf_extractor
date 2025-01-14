using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Services
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

        internal static void WriteEntry(string v, string message)
        {
            string logMessage = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}: {message}";            
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
