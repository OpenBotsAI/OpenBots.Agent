using OpenBots.Agent.Core.Model;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System;
using System.Net;

namespace OpenBots.Executor.Utilities
{
    /// <summary>
    /// Handles functionality for logging to files
    /// </summary>
    public class Logging
    {
        public Logger CreateFileLogger(string filePath, RollingInterval logInterval,
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
                        .Enrich.WithProperty("ProcessId", executionParams.ProcessId)
                        .Enrich.WithProperty("ProcessName", executionParams.ProcessName)
                        .Enrich.WithProperty("AgentId", executionParams.ServerConnectionSettings.AgentId)
                        .Enrich.WithProperty("AgentName", executionParams.ServerConnectionSettings.AgentName)
                        .Enrich.WithProperty("MachineName", executionParams.ServerConnectionSettings.DNSHost)
                        .MinimumLevel.ControlledBy(levelSwitch)
                        .WriteTo.Http(uri)
                        .CreateLogger();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Logger CreateSignalRLogger(JobExecutionParams executionParams, string url, string logHub = "LogHub", string[] logGroupNames = null,
            string[] logUserIds = null, LogEventLevel minLogLevel = LogEventLevel.Verbose)
        {
            try
            {
                var levelSwitch = new LoggingLevelSwitch();
                levelSwitch.MinimumLevel = minLogLevel;

                return new LoggerConfiguration()
                        .Enrich.WithProperty("JobId", executionParams.JobId)
                        .Enrich.WithProperty("ProcessId", executionParams.ProcessId)
                        .Enrich.WithProperty("ProcessName", executionParams.ProcessName)
                        .Enrich.WithProperty("AgentId", executionParams.ServerConnectionSettings.AgentId)
                        .Enrich.WithProperty("AgentName", executionParams.ServerConnectionSettings.AgentName)
                        .Enrich.WithProperty("MachineName", executionParams.ServerConnectionSettings.DNSHost)
                        .MinimumLevel.ControlledBy(levelSwitch)
                        .WriteTo.SignalRClient(url,
                                               hub: logHub, // default is LogHub
                                               groupNames: logGroupNames, // default is null
                                               userIds: logUserIds)// default is null
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


    }
}
