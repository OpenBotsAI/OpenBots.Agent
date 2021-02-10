using OpenBots.Agent.Core.Utilities;
using Serilog.Core;
using Serilog.Events;
using System.IO;

namespace OpenBots.Service.Client.Manager.Logs
{
    public class FileLogger
    {
        private Logger _logger;
        
        public FileLogger()
        {
        }

        public void Initialize(string agentDataDirectoryPath)
        {
            if(_logger == null)
            {
                string logFilePath = Path.Combine(
                agentDataDirectoryPath,
                "Logs",
                "log.txt"
                );

                string logsDirectory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(logsDirectory))
                    Directory.CreateDirectory(logsDirectory);

                _logger = new Logging().CreateStandardFileLogger(logFilePath, LogEventLevel.Information);
            }
        }

        public void LogEvent(string eventName, string message, LogEventLevel eventLevel = LogEventLevel.Information)
        {
            var logMessage = $"Event Name: {eventName} | Log Message: {message}";

            switch (eventLevel)
            {
                case LogEventLevel.Verbose:
                    _logger.Verbose(logMessage);
                    break;
                case LogEventLevel.Debug:
                    _logger.Debug(logMessage);
                    break;
                case LogEventLevel.Information:
                    _logger.Information(logMessage);
                    break;
                case LogEventLevel.Warning:
                    _logger.Warning(logMessage);
                    break;
                case LogEventLevel.Error:
                    _logger.Error(logMessage);
                    break;
                case LogEventLevel.Fatal:
                    _logger.Fatal(logMessage);
                    break;
            }
        }
    }
}
