using Autofac;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Engine;
using OpenBots.Executor.Utilities;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenBots.Executor
{
    public class EngineHandler
    {
        private IContainer _container;
        public EngineHandler()
        {
        }

        public void LoadProjectAssemblies(List<string> projectAssemblies)
        {
            var builder = AssembliesManager.LoadBuilder(projectAssemblies);
            _container = builder.Build();
        }

        public void ExecuteScript(JobExecutionParams executionParams)
        {
            var engine = new AutomationEngineInstance(GetLogger(executionParams));
            engine.ExecuteScriptSync(executionParams.MainFilePath, executionParams.ProjectDirectoryPath);
        }

        private Logger GetLogger(JobExecutionParams executionParams)
        {
            Logger logger = null;

            // Get Minimum Log Level
            LogEventLevel minLogLevel;
            Enum.TryParse(executionParams.ServerConnectionSettings.TracingLevel, out minLogLevel);

            // Get Log Sink Type (File, HTTP, SignalR)
            SinkType sinkType;
            Enum.TryParse(executionParams.ServerConnectionSettings.SinkType, out sinkType);

            switch (sinkType)
            {
                case SinkType.File:
                    string logFile = Path.Combine(executionParams.ServerConnectionSettings.LoggingValue1);
                    logger = new Logging().CreateFileLogger(logFile, Serilog.RollingInterval.Day, minLogLevel);

                    break;
                case SinkType.Http:
                    logger = new Logging().CreateHTTPLogger(executionParams,
                        executionParams.ServerConnectionSettings.LoggingValue1, minLogLevel);

                    break;
                case SinkType.SignalR:

                    logger = new Logging().CreateSignalRLogger(executionParams,
                        executionParams.ServerConnectionSettings.LoggingValue1,
                        executionParams.ServerConnectionSettings.LoggingValue2,
                        executionParams.ServerConnectionSettings.LoggingValue3.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                        executionParams.ServerConnectionSettings.LoggingValue4.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                        minLogLevel);

                    break;
            }

            return logger;
        }
    }
}
