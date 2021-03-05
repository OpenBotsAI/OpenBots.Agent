using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.IO;

namespace OpenBots.Agent.Core.Utilities
{
    /// <summary>
    /// Handles functionality for logging to files
    /// </summary>
    public class Logging
    {
        public Logger CreateFileLogger(string filePath, RollingInterval logInterval = RollingInterval.Day,
            LogEventLevel minLogLevel = LogEventLevel.Verbose)
        {
            try
            {
                var levelSwitch = new LoggingLevelSwitch();
                levelSwitch.MinimumLevel = minLogLevel;

                return new LoggerConfiguration()
                        .Enrich.WithProperty("JobId", Guid.NewGuid())
                        .MinimumLevel.ControlledBy(levelSwitch)
                        .WriteTo.File(filePath, rollingInterval: logInterval)
                        .CreateLogger();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Logger CreateHTTPLogger(JobExecutionParams executionParams, string uri, LogEventLevel minLogLevel = LogEventLevel.Verbose)
        {
            try
            {
                var levelSwitch = new LoggingLevelSwitch();
                levelSwitch.MinimumLevel = minLogLevel;

                return new LoggerConfiguration()
                        .Enrich.WithProperty("JobId", executionParams.JobId)
                        .Enrich.WithProperty("AutomationId", executionParams.AutomationId)
                        .Enrich.WithProperty("AutomationName", executionParams.AutomationName)
                        .Enrich.WithProperty("AgentId", executionParams.ServerConnectionSettings.AgentId)
                        .Enrich.WithProperty("AgentName", executionParams.ServerConnectionSettings.AgentName)
                        .Enrich.WithProperty("MachineName", executionParams.ServerConnectionSettings.MachineName)
                        .MinimumLevel.ControlledBy(levelSwitch)
                        .WriteTo.Http(requestUri: uri, period: TimeSpan.FromSeconds((double)executionParams.ServerConnectionSettings.JobsLoggingInterval))
                        .CreateLogger();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Logger CreateJsonFileLogger(string jsonFilePath, RollingInterval logInterval,
            LogEventLevel minLogLevel = LogEventLevel.Verbose)
        {
            try
            {
                var levelSwitch = new LoggingLevelSwitch();
                levelSwitch.MinimumLevel = minLogLevel;

                return new LoggerConfiguration()
                        .Enrich.WithProperty("JobId", Guid.NewGuid())
                        .MinimumLevel.ControlledBy(levelSwitch)
                        .WriteTo.File(new CompactJsonFormatter(), jsonFilePath, rollingInterval: logInterval)
                        .CreateLogger();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Logger CreateStandardFileLogger(string filePath, LogEventLevel minLogLevel = LogEventLevel.Information)
        {
            try
            {
                var levelSwitch = new LoggingLevelSwitch();
                levelSwitch.MinimumLevel = minLogLevel;

                return new LoggerConfiguration()
                        .MinimumLevel.ControlledBy(levelSwitch)
                        .WriteTo.File(filePath, rollingInterval: RollingInterval.Day)
                        .CreateLogger();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Logger GetLogger(JobExecutionParams executionParams)
        {
            Logger logger = null;

            // Get Minimum Log Level
            LogEventLevel minLogLevel;
            Enum.TryParse(executionParams.ServerConnectionSettings.TracingLevel, out minLogLevel);

            // Get Log Sink Type (File, HTTP)
            SinkType sinkType;
            Enum.TryParse(executionParams.ServerConnectionSettings.SinkType, out sinkType);

            switch (sinkType)
            {
                case SinkType.File:
                    string logFile = Path.Combine(executionParams.ServerConnectionSettings.LoggingValue1);
                    logger = CreateFileLogger(logFile, Serilog.RollingInterval.Day, minLogLevel);

                    break;
                case SinkType.Http:
                    logger = CreateHTTPLogger(executionParams,
                        executionParams.ServerConnectionSettings.LoggingValue1, minLogLevel);

                    break;
            }

            return logger;
        }
    }
}
