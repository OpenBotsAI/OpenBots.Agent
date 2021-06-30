using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Nuget;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Server.SDK.HelperMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class AttendedExecutionManager
    {
        private ExecutionManager _executionManager;
        private AuthMethods _authMethods;

        public AttendedExecutionManager(ExecutionManager executionManager)
        {
            _executionManager = executionManager;
        }

        public bool ExecuteTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            _authMethods = new AuthMethods(
                    settings.ServerType,
                    settings.OrganizationName,
                    settings.ServerURL,
                    settings.AgentUsername,
                    settings.AgentPassword,
                    settings.UserName,
                    settings.DNSHost);

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

                        // Authenticate Agent
                        var userInfo = _authMethods.GetUserInfo();

                        var automation = AutomationMethods.GetAutomations(userInfo, filter).Items.FirstOrDefault();
                        mainScriptFilePath = AutomationManager.DownloadAndExtractAutomation(_authMethods, automation, string.Empty, settings.DNSHost, settings.UserName, out projectDirectoryPath, out configFilePath);
                    }
                    else
                    {
                        // projectPackage is "Path" of the Project Package here
                        mainScriptFilePath = AutomationManager.GetMainScriptFilePath(projectPackage, out configFilePath);
                    }
                    
                    projectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath);
                    string projectType = JObject.Parse(File.ReadAllText(configFilePath))["ProjectType"].ToString();
                    var automationType = (AutomationType)Enum.Parse(typeof(AutomationType), projectType);
                    switch (automationType)
                    {
                        case AutomationType.OpenBots:
                            NugetPackageManager.InstallProjectDependencies(configFilePath, settings.DNSHost, settings.UserName);
                            var assembliesList = NugetPackageManager.LoadPackageAssemblies(configFilePath, settings.DNSHost, settings.UserName);
                            RunOpenBotsAutomation(mainScriptFilePath, Path.GetFileNameWithoutExtension(projectPackage), settings, assembliesList);
                            break;

                        case AutomationType.Python:
                            RunPythonAutomation(mainScriptFilePath, Path.GetFileNameWithoutExtension(projectPackage), settings);
                            break;

                        case AutomationType.TagUI:
                            RunTagUIAutomation(mainScriptFilePath, Path.GetFileNameWithoutExtension(projectPackage), settings, projectDirectoryPath);
                            break;

                        case AutomationType.CSScript:
                            RunCSharpAutomation(mainScriptFilePath, Path.GetFileNameWithoutExtension(projectPackage), settings);
                            break;

                        default:
                            throw new NotImplementedException($"Specified automation type \"{automationType}\" is not supported in the OpenBots Agent.");
                    }

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

        private void RunOpenBotsAutomation(string mainScriptFilePath, string projectName, ServerConnectionSettings settings, List<string> projectDependencies)
        {
            settings.LoggingValue1 = GetLogsFilePath(settings.LoggingValue1, AutomationType.OpenBots, projectName);
            var executionParams = GetExecutionParams(mainScriptFilePath, settings, projectDependencies);
            var userInfo = new MachineCredential
            {
                Domain = settings.DNSHost,
                UserName = settings.UserName
            };

            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";

            // Run Automation
            RunAutomation(cmdLine, userInfo, settings);
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

        private void RunPythonAutomation(string mainScriptFilePath, string projectName, ServerConnectionSettings settings)
        {
            string pythonExecutable = _executionManager.GetPythonPath(settings.UserName, "");
            string projectDir = Path.GetDirectoryName(mainScriptFilePath);

            string commandsBatch = $"\"{pythonExecutable}\" -m pip install --upgrade pip && " +
                $"\"{pythonExecutable}\" -m pip install --user virtualenv && " +
                $"\"{pythonExecutable}\" -m venv \"{Path.Combine(projectDir, ".env3")}\" && " +
                $"\"{Path.Combine(projectDir, ".env3", "Scripts", "activate.bat")}\" && " +
                (File.Exists(Path.Combine(projectDir, "requirements.txt")) ? $"\"{pythonExecutable}\" -m pip install -r \"{Path.Combine(projectDir, "requirements.txt")}\" & " : "") +
                $"\"{pythonExecutable}\" \"{mainScriptFilePath}\" && " +
                $"deactivate";

            string batchFilePath = Path.Combine(projectDir, projectName + ".bat");
            File.WriteAllText(batchFilePath, commandsBatch);
            string logsFilePath = GetLogsFilePath(settings.LoggingValue1, AutomationType.Python, projectName);

            string cmdLine = $"\"{batchFilePath}\" > \"{logsFilePath}\"";
            var userInfo = new MachineCredential
            {
                Domain = settings.DNSHost,
                UserName = settings.UserName
            };

            RunAutomation(cmdLine, userInfo, settings);

            return;
        }

        private void RunTagUIAutomation(string mainScriptFilePath, string projectName, ServerConnectionSettings settings, string executionDirPath)
        {
            string exePath = _executionManager.GetFullPathFromWindows("tagui", settings.DNSHost, settings.UserName);
            if (exePath == null)
                throw new Exception("TagUI installation was not detected on the machine. Please perform the installation as outlined in the official documentation.");

            // Create "tagui_logging" file for generating logs file
            var logFilePath = Path.Combine(Directory.GetParent(exePath).FullName, "tagui_logging");
            if (!File.Exists(logFilePath))
                File.Create(Path.Combine(Directory.GetParent(exePath).FullName, "tagui_logging"));

            // Copy Script Folder/Files to ".\tagui\flows" Directory
            var mainScriptPath = _executionManager.CopyTagUIAutomation(exePath, mainScriptFilePath, ref executionDirPath);
            var logsFilePath = GetLogsFilePath(settings.LoggingValue1, AutomationType.TagUI, projectName);

            string cmdLine = $"C:\\Windows\\System32\\cmd.exe /C tagui \"{mainScriptPath}\" > \"{logsFilePath}\"";
            var userInfo = new MachineCredential
            {
                Domain = settings.DNSHost,
                UserName = settings.UserName
            };

            RunAutomation(cmdLine, userInfo, settings);

            // Delete TagUI Execution Directory
            Directory.Delete(executionDirPath, true);

            return;
        }

        private void RunCSharpAutomation(string mainScriptFilePath, string projectName, ServerConnectionSettings settings)
        {
            string exePath = _executionManager.GetFullPathFromWindows("cscs.exe", settings.DNSHost, settings.UserName);
            if (exePath == null)
                throw new Exception("CS-Script installation was not detected on the machine. Please perform the installation as outlined in the official documentation.");

            var logsFilePath = GetLogsFilePath(settings.LoggingValue1, AutomationType.CSScript, projectName);
            string cmdLine = $"C:\\Windows\\System32\\cmd.exe /C cscs \"{mainScriptFilePath}\" > \"{logsFilePath}\"";
            var userInfo = new MachineCredential
            {
                Domain = settings.DNSHost,
                UserName = settings.UserName
            };

            RunAutomation(cmdLine, userInfo, settings);

            return;
        }

        private void RunAutomation(string commandLine, MachineCredential machineCredential, ServerConnectionSettings settings)
        {
            Executor executor = new Executor(null);
            executor.RunAutomation(commandLine, machineCredential, settings);
        }

        private string GetLogsFilePath(string logsDirectory, AutomationType automationType, string projectName)
        {
            string logsFilePath = $"{Path.Combine(logsDirectory, automationType.ToString(), projectName + DateTime.Now.ToString("MMddyyyyhhmmss") + ".txt")}";
            if (!Directory.Exists(Path.GetDirectoryName(logsFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(logsFilePath));

            return logsFilePath;
        }

    }
}
