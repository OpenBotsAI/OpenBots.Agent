using Autofac;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Nuget;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Executor.Model;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OpenBots.Executor
{
    public class EngineHandler
    {
        private IContainer _container;
        private Assembly _engineAssembly;
        private EngineAssemblyInfo _assemblyInfo;
        public EngineHandler()
        {
            _assemblyInfo = new EngineAssemblyInfo();
        }

        public void LoadProjectAssemblies(List<string> projectAssemblies)
        {
            var builder = AppDomainSetupManager.LoadBuilder(projectAssemblies);
            _container = builder.Build();
            //var engineAssemblyFilePath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, _assemblyInfo.FileName).FirstOrDefault();
            //if (engineAssemblyFilePath != null)
            //    _engineAssembly = Assembly.LoadFrom(engineAssemblyFilePath);
            //else
            //    throw new Exception($"Assembly path for {_assemblyInfo.FileName} not found.");
        }

        public void ExecuteScript(JobExecutionParams executionParams)
        {
            //Type t = _engineAssembly.GetType(_assemblyInfo.ClassName);
            Type t = AppDomainSetupManager.GetTypeByName(_container, _assemblyInfo.ClassName);

            var methodInfo = t.GetMethod(_assemblyInfo.MethodName, new Type[] { typeof(string), typeof(string) });
            if (methodInfo == null)
            {
                throw new Exception($"No method exists with name {_assemblyInfo.MethodName} within Type {_assemblyInfo.ClassName}");
            }

            //
            // Specify paramters for the constructor: 'AutomationEngineInstance(bool isRemoteExecution = false)'
            //
            object[] engineParams = new object[1];
            engineParams[0] = GetLogger(executionParams);
            //
            // Create instance of Class "AutomationEngineInstance".
            //
            var engine = Activator.CreateInstance(t, engineParams);

            //
            // Specify parameters for the method we will be invoking: 'void ExecuteScriptAsync(string filePath, string projectPath)'
            //
            object[] parameters = new object[2];
            parameters[0] = executionParams.MainFilePath;                    // 'filePath' parameter
            parameters[1] = executionParams.ProjectDirectoryPath;            // 'projectPath' parameter

            //
            // 6. Invoke method 'void ExecuteScriptAsync(string filePath, string projectPath)'
            //
            methodInfo.Invoke(engine, parameters);
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
