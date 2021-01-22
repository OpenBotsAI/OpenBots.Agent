
using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Nuget;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Server;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.Win32;
using System.Security.Principal;
using System.Reflection;
using OpenBots.Agent.Core.Enums;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class ExecutionManager
    {
        public bool IsEngineBusy { get; set; } = false;
        private bool _isSuccessfulExecution = false;
        private Timer _newJobsCheckTimer;
        private AutomationExecutionLog _executionLog;

        public event EventHandler JobFinishedEvent;
        public static ExecutionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ExecutionManager();

                return instance;
            }
        }
        private static ExecutionManager instance;

        private ExecutionManager()
        {
        }

        public void StartNewJobsCheckTimer()
        {
            //handle for reinitialization
            if (_newJobsCheckTimer != null)
            {
                _newJobsCheckTimer.Elapsed -= NewJobsCheckTimer_Elapsed;
            }

            _newJobsCheckTimer = new Timer();
            _newJobsCheckTimer.Interval = 3000;
            _newJobsCheckTimer.Elapsed += NewJobsCheckTimer_Elapsed;
            _newJobsCheckTimer.Enabled = true;
        }
        public void StopNewJobsCheckTimer()
        {
            if (_newJobsCheckTimer != null)
            {
                _newJobsCheckTimer.Enabled = false;
                _newJobsCheckTimer.Elapsed -= NewJobsCheckTimer_Elapsed;
            }
        }

        // To Check if JobQueue has a New Job to be executed
        private void NewJobsCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // If Jobs Queue is not Empty & No (Server or Attended) Job is being executed
                if (!JobsQueueManager.Instance.IsQueueEmpty() && !IsEngineBusy)
                {
                    SetEngineStatus(true);
                    ExecuteJob();
                    SetEngineStatus(false);
                }
            }
            catch (Exception ex)
            {
                // Log Event
                FileLogger.Instance.LogEvent("Job Execution", $"Error occurred while executing the job; ErrorMessage = {ex.ToString()}",
                    LogEventLevel.Error);

                try
                {
                    var job = JobsQueueManager.Instance.DequeueJob();

                    // Update Automation Execution Log (Execution Failure)
                    if (_executionLog != null)
                    {
                        _executionLog.Status = "Job has failed";
                        _executionLog.HasErrors = true;
                        _executionLog.ErrorMessage = ex.Message;
                        _executionLog.ErrorDetails = ex.ToString();
                        ExecutionLogsAPIManager.UpdateExecutionLog(AuthAPIManager.Instance, _executionLog);
                    }

                    // Update Job Status (Failed)
                    JobsAPIManager.UpdateJobStatus(AuthAPIManager.Instance, job.AgentId.ToString(), job.Id.ToString(),
                    JobStatusType.Failed, new JobErrorViewModel(
                        ex.Message,
                        ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty,
                        ExceptionSerializer.Serialize(ex))
                    );
                }
                catch (Exception exception)
                {
                    // Log Event
                    FileLogger.Instance.LogEvent("Job Execution", $"Error occurred while updating status on job failure; " +
                        $"ErrorMessage = {exception}", LogEventLevel.Error);

                    throw;
                }
                _isSuccessfulExecution = false;
                HeartBeatManager.Instance.Heartbeat.LastReportedMessage = "Automation execution failed";
                SetEngineStatus(false);
            }
        }

        private void ExecuteJob()
        {
            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Job execution started");

            // Peek Job
            var job = JobsQueueManager.Instance.PeekJob();

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to fetch Automation Detail");

            // Get Automation Info
            var automation = AutomationsAPIManager.GetAutomation(AuthAPIManager.Instance, job.AutomationId.ToString());

            // Update LastReportedMessage and LastReportedWork
            HeartBeatManager.Instance.Heartbeat.LastReportedMessage = "Automation execution started";
            HeartBeatManager.Instance.Heartbeat.LastReportedWork = automation.Name;

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to download/retrieve Automation");

            string connectedUserName = ConnectionSettingsManager.Instance.ConnectionSettings.UserName;

            // Download Automation and Extract Files and Return File Paths of ProjectConfig and MainScript 
            automation.AutomationEngine = string.IsNullOrEmpty(automation.AutomationEngine) ? "OpenBots" : automation.AutomationEngine;
            string configFilePath;
            var mainScriptFilePath = AutomationManager.DownloadAndExtractAutomation(automation, connectedUserName, out configFilePath);

            // Install Project Dependencies
            List<string> assembliesList = null;
            if (automation.AutomationEngine == "OpenBots")
            {
                NugetPackageManager.InstallProjectDependencies(configFilePath, connectedUserName);
                assembliesList = NugetPackageManager.LoadPackageAssemblies(configFilePath, connectedUserName);
            }

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to update Job Status (Pre-execution)");

            // Create Automation Execution Log (Execution Started)
            _executionLog = ExecutionLogsAPIManager.CreateExecutionLog(AuthAPIManager.Instance, new AutomationExecutionLog(
                null, false, null, DateTime.UtcNow, null, null, null, null, null, job.Id, job.AutomationId, job.AgentId,
                DateTime.UtcNow, null, null, null, "Job has started processing"));

            // Update Job Status (InProgress)
            JobsAPIManager.UpdateJobStatus(AuthAPIManager.Instance, job.AgentId.ToString(), job.Id.ToString(),
                JobStatusType.InProgress, new JobErrorViewModel());

            // Update Job Start Time
            JobsAPIManager.UpdateJobPatch(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/startTime", Value = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")}
                });

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to execute process");

            AgentViewModel agent = AgentsAPIManager.GetAgent(AuthAPIManager.Instance, job.AgentId.ToString());
            var userCredential = CredentialsAPIManager.GetCredentials(AuthAPIManager.Instance, agent.CredentialId.ToString());
            MachineCredential credential = new MachineCredential
            {
                Name = userCredential.Name,
                Domain = userCredential.Domain,
                UserName = userCredential.UserName,
                PasswordSecret = userCredential.PasswordSecret
            };

            // Run Automation
            RunAutomation(job, automation, credential, mainScriptFilePath, assembliesList);

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Job execution completed");

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to update Job Status (Post-execution)");

            // Update Job End Time
            JobsAPIManager.UpdateJobPatch(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/endTime", Value = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")},
                });

            // Delete Automation Files Directory
            Directory.Delete(Path.GetDirectoryName(mainScriptFilePath), true);

            // Update Automation Execution Log (Execution Finished)
            _executionLog.CompletedOn = DateTime.UtcNow;
            _executionLog.Status = "Job has finished processing";
            ExecutionLogsAPIManager.UpdateExecutionLog(AuthAPIManager.Instance, _executionLog);

            // Update Job Status (Completed)
            JobsAPIManager.UpdateJobStatus(AuthAPIManager.Instance, job.AgentId.ToString(), job.Id.ToString(),
                JobStatusType.Completed, new JobErrorViewModel());

            FileLogger.Instance.LogEvent("Job Execution", "Job status updated. Removing from queue.");

            // Dequeue the Job
            JobsQueueManager.Instance.DequeueJob();

            _isSuccessfulExecution = true;
            HeartBeatManager.Instance.Heartbeat.LastReportedMessage = "Automation execution completed";
        }
        private void RunAutomation(Job job, Automation automation, MachineCredential machineCredential,
            string mainScriptFilePath, List<string> projectDependencies)
        {
            try
            {
                if (automation.AutomationEngine == "")
                    automation.AutomationEngine = "OpenBots";

                switch (automation.AutomationEngine.ToString())
                {
                    case "OpenBots":
                        RunOpenBotsAutomation(job, automation, machineCredential, mainScriptFilePath, projectDependencies);
                        break;

                    case "Python":
                        RunPythonAutomation(job, machineCredential, mainScriptFilePath);
                        break;
                    default:
                        throw new NotImplementedException($"Specified execution engine \"{automation.AutomationEngine}\" is not implemented on the OpenBots Agent.");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void RunOpenBotsAutomation(Job job, Automation automation, MachineCredential machineCredential, string mainScriptFilePath, List<string> projectDependencies)
        {
            var executionParams = GetExecutionParams(job, automation, mainScriptFilePath, projectDependencies);
            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";

            // launch the Executor
            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, machineCredential, out procInfo);
            return;
        }

        private void RunPythonAutomation(Job job, MachineCredential machineCredential, string mainScriptFilePath)
        {
            string projectDir = Path.GetDirectoryName(mainScriptFilePath);
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pythonExecutable = GetPythonPath(machineCredential.UserName, "");
            string cmdLine = $"powershell.exe -File \"{assemblyPath}\\Executors\\PythonExecutor.ps1\" \"{pythonExecutable}\" \"{projectDir}\" \"{mainScriptFilePath}\"";

            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, machineCredential, out procInfo);

            return;
        }

        public void SetEngineStatus(bool isBusy)
        {
            IsEngineBusy = isBusy;
            if (IsEngineBusy)
                HeartBeatManager.Instance.Heartbeat.LastReportedStatus = AgentStatus.Busy.ToString();
            else
            {
                HeartBeatManager.Instance.Heartbeat.LastReportedStatus = AgentStatus.Available.ToString();
                OnJobFinishedEvent(EventArgs.Empty);
            }
        }

        protected virtual void OnJobFinishedEvent(EventArgs e)
        {
            if (_isSuccessfulExecution)
                JobFinishedEvent?.Invoke(this, e);
        }

        private string GetExecutionParams(Job job, Automation automation, string mainScriptFilePath, List<string> projectDependencies)
        {
            var executionParams = new JobExecutionParams()
            {
                JobId = job.Id.ToString(),
                AutomationId = automation.Id.ToString(),
                AutomationName = automation.Name,
                MainFilePath = mainScriptFilePath,
                ProjectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath),
                ProjectDependencies = projectDependencies,
                ServerConnectionSettings = ConnectionSettingsManager.Instance.ConnectionSettings
            };
            var paramsJsonString = JsonConvert.SerializeObject(executionParams);
            return DataFormatter.CompressString(paramsJsonString);
        }

        private static string GetPythonPath(string username, string requiredVersion = "")
        {
            var possiblePythonLocations = new List<string>()
            {
                @"HKLM\SOFTWARE\Python\PythonCore\",
                @"HKLM\SOFTWARE\Wow6432Node\Python\PythonCore\"
            };

            try
            {
                NTAccount f = new NTAccount(username);
                SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
                string sidString = s.ToString();
                possiblePythonLocations.Add($@"HKU\{sidString}\SOFTWARE\Python\PythonCore\");
            }
            catch
            {
                throw new Exception("Unabled to retrieve SID for provided user credentials.");
            }

            Version requestedVersion = new Version(requiredVersion == "" ? "0.0.1" : requiredVersion);

            //Version number, install path
            Dictionary<Version, string> pythonLocations = new Dictionary<Version, string>();

            foreach (string possibleLocation in possiblePythonLocations)
            {
                var regVals = possibleLocation.Split(new[] { '\\' }, 2);
                var rootKey = (regVals[0] == "HKLM" ? RegistryHive.LocalMachine : RegistryHive.Users);
                var regView = (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                var hklm = RegistryKey.OpenBaseKey(rootKey, regView);
                RegistryKey theValue = hklm.OpenSubKey(regVals[1]);

                if (theValue == null)
                    continue;

                foreach (var version in theValue.GetSubKeyNames())
                {
                    RegistryKey productKey = theValue.OpenSubKey(version);
                    if (productKey != null)
                    {
                        try
                        {
                            string pythonExePath = productKey.OpenSubKey("InstallPath").GetValue("ExecutablePath").ToString();
                            if (pythonExePath != null && pythonExePath != "")
                            {
                                pythonLocations.Add(Version.Parse(version), pythonExePath);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            if (pythonLocations.Count == 0)
                throw new Exception("No installed Python versions found.");

            int max = pythonLocations.Max(x => x.Key.CompareTo(requestedVersion));
            requestedVersion = pythonLocations.First(x => x.Key.CompareTo(requestedVersion) == max).Key;

            if (pythonLocations.ContainsKey(requestedVersion))
            {
                return pythonLocations[requestedVersion];
            }
            else
            {
                throw new Exception($"Required Python version [{requiredVersion}] or higher was not found on the machine.");
            }
        }
    }
}
