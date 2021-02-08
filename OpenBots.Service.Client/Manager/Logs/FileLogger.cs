using OpenBots.Agent.Core.Utilities;
using Serilog.Core;
using Serilog.Events;
using System.IO;

namespace OpenBots.Service.Client.Manager.Logs
{
    public class FileLogger
    {
        private Logger logger;
        public static FileLogger Instance
        {
            get
            {
                if (instance == null)
                    instance = new FileLogger();

                return instance;
            }
        }
        private static FileLogger instance;

        private FileLogger()
        {
        }

        public void Initialize(string agentDataDirectoryPath)
        {
            string logFilePath = Path.Combine(
                agentDataDirectoryPath,
                "Logs",
                "log.txt"
                );

            string logsDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logsDirectory))
                Directory.CreateDirectory(logsDirectory);

            logger = new Logging().CreateStandardFileLogger(logFilePath, LogEventLevel.Information);
        }

        public void LogEvent(string eventName, string message, LogEventLevel eventLevel = LogEventLevel.Information)
        {
            var logMessage = $"Event Name: {eventName} | Log Message: {message}";

            switch (eventLevel)
            {
                case LogEventLevel.Verbose:
                    logger.Verbose(logMessage);
                    break;
                case LogEventLevel.Debug:
                    logger.Debug(logMessage);
                    break;
                case LogEventLevel.Information:
                    logger.Information(logMessage);
                    break;
                case LogEventLevel.Warning:
                    logger.Warning(logMessage);
                    break;
                case LogEventLevel.Error:
                    logger.Error(logMessage);
                    break;
                case LogEventLevel.Fatal:
                    logger.Fatal(logMessage);
                    break;
            }
        }
    }
}
