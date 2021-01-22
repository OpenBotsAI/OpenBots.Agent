using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Nuget;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class AttendedExecutionManager
    {
        public static AttendedExecutionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AttendedExecutionManager();

                return instance;
            }
        }
        private static AttendedExecutionManager instance;

        private AttendedExecutionManager()
        {
        }

        public bool ExecuteTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            if (!ExecutionManager.Instance.IsEngineBusy)
            {
                bool isSuccessful;
                string projectDirectoryPath, configFilePath, mainScriptFilePath;
                projectDirectoryPath = configFilePath = mainScriptFilePath = string.Empty;
                try
                {
                    ExecutionManager.Instance.SetEngineStatus(true);
                    if (isServerAutomation)
                    {
                        // projectPackage is "Name" of the Project Package here
                        string filter = $"originalPackageName eq '{projectPackage}'";
                        var automation = AutomationsAPIManager.GetAutomations(AuthAPIManager.Instance, filter).Items.FirstOrDefault();
                        mainScriptFilePath = AutomationManager.DownloadAndExtractAutomation(automation, settings.UserName, out configFilePath);
                    }
                    else
                    {
                        // projectPackage is "Path" of the Project Package here
                        mainScriptFilePath = AutomationManager.GetMainScriptFilePath(projectPackage, out configFilePath);
                    }
                    
                    projectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath);
                    NugetPackageManager.InstallProjectDependencies(configFilePath, settings.UserName);
                    var assembliesList = NugetPackageManager.LoadPackageAssemblies(configFilePath, settings.UserName);

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

                    ExecutionManager.Instance.SetEngineStatus(false);
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
