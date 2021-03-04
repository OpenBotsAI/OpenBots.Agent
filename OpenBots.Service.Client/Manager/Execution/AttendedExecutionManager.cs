using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Nuget;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Service.API.Client;
using OpenBots.Service.Client.Manager.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class AttendedExecutionManager
    {
        private ExecutionManager _executionManager;
        private AuthAPIManager _authAPIManager;
        public AttendedExecutionManager(ExecutionManager executionManager, AuthAPIManager authAPIManager)
        {
            _executionManager = executionManager;
            _authAPIManager = authAPIManager;

            _authAPIManager.ConfigurationUpdatedEvent += OnConfigurationUpdate; 
        }

        private void OnConfigurationUpdate(object sender, Configuration configuration)
        {
            _authAPIManager.Configuration = configuration;
        }

        public bool ExecuteTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            if (!_executionManager.IsEngineBusy)
            {
                bool isSuccessful;
                string projectDirectoryPath, configFilePath, mainScriptFilePath;
                projectDirectoryPath = configFilePath = mainScriptFilePath = string.Empty;
                try
                {
                    _executionManager.SetEngineStatus(true);
                    if (isServerAutomation)
                    {
                        // projectPackage is "Name" of the Project Package here
                        string filter = $"originalPackageName eq '{projectPackage}'";
                        var automation = AutomationsAPIManager.GetAutomations(_authAPIManager, filter).Data?.Items.FirstOrDefault();
                        mainScriptFilePath = AutomationManager.DownloadAndExtractAutomation(_authAPIManager, automation, string.Empty, settings.DNSHost, settings.UserName, out projectDirectoryPath, out configFilePath);
                    }
                    else
                    {
                        // projectPackage is "Path" of the Project Package here
                        mainScriptFilePath = AutomationManager.GetMainScriptFilePath(projectPackage, out configFilePath);
                    }
                    
                    projectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath);
                    NugetPackageManager.InstallProjectDependencies(configFilePath, settings.DNSHost, settings.UserName);
                    var assembliesList = NugetPackageManager.LoadPackageAssemblies(configFilePath, settings.DNSHost, settings.UserName);

                    RunAttendedAutomation(mainScriptFilePath, settings, assembliesList);

                    isSuccessful = true;
                }
                catch (Exception)
                {
                    isSuccessful = false;
                }
                finally
                {
                    // Delete Project Directory
                    if (Directory.Exists(projectDirectoryPath))
                        Directory.Delete(projectDirectoryPath, true);

                    _executionManager.SetEngineStatus(false);
                }

                return isSuccessful;
            }
            return false;
        }

        private void RunAttendedAutomation(string mainScriptFilePath, ServerConnectionSettings settings, List<string> projectDependencies)
        {
            var executionParams = GetExecutionParams(mainScriptFilePath, settings, projectDependencies);
            var userInfo = new MachineCredential
            {
                Domain = settings.DNSHost,
                UserName = settings.UserName
            };

            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";

            // launch the Executor
            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, userInfo, out procInfo);
        }

        private string GetExecutionParams(string mainScriptFilePath, ServerConnectionSettings settings, List<string> projectDependencies)
        {
            var executionParams = new JobExecutionParams()
            {
                MainFilePath = mainScriptFilePath,
                ProjectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath),
                ProjectDependencies = projectDependencies,
                ServerConnectionSettings = settings
            };
            var paramsJsonString = JsonConvert.SerializeObject(executionParams);
            return DataFormatter.CompressString(paramsJsonString);
        }
    }
}
