using OpenBots.Executor.Model;
using OpenBots.Executor.Utils;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenBots.Executor
{
    public class EngineHandler
    {
        private Assembly _engineAssembly;
        private EngineAssemblyInfo _assemblyInfo;
        public EngineHandler()
        {
            _assemblyInfo = new EngineAssemblyInfo();
            LoadEngineAssembly();
        }

        private void LoadEngineAssembly()
        {
            var engineAssemblyFilePath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, _assemblyInfo.FileName).FirstOrDefault();
            if (engineAssemblyFilePath != null)
                _engineAssembly = Assembly.LoadFrom(engineAssemblyFilePath);
            else
                throw new Exception($"Assembly path for {_assemblyInfo.FileName} not found.");
        }

        public void ExecuteScript(string mainScriptPath)
        {
            string projectDirectory = Path.GetDirectoryName(mainScriptPath);
            string logFile = Path.Combine(projectDirectory, "logs", "OpenBots-Logs.txt");
            Type t = _engineAssembly.GetType(_assemblyInfo.ClassName);

            var methodInfo = t.GetMethod(_assemblyInfo.MethodName, new Type[] { typeof(string), typeof(string) });
            if (methodInfo == null)
            {
                throw new Exception($"No method exists with name {_assemblyInfo.MethodName} within Type {_assemblyInfo.ClassName}");
            }

            //
            // Specify paramters for the constructor: 'AutomationEngineInstance(bool isRemoteExecution = false)'
            //
            object[] engineParams = new object[1];
            engineParams[0] = new Logging().CreateFileLogger(logFile, Serilog.RollingInterval.Day);
            //
            // Create instance of Class "AutomationEngineInstance".
            //
            var engine = Activator.CreateInstance(t, engineParams);

            //
            // Specify parameters for the method we will be invoking: 'void ExecuteScriptAsync(string filePath, string projectPath)'
            //
            object[] parameters = new object[2];
            parameters[0] = mainScriptPath;            // 'filePath' parameter
            parameters[1] = projectDirectory;            // 'projectPath' parameter

            //
            // 6. Invoke method 'void ExecuteScriptAsync(string filePath, string projectPath)'
            //
            methodInfo.Invoke(engine, parameters);
        }
    }
}
