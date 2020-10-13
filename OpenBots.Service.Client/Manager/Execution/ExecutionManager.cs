
using Newtonsoft.Json.Linq;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Timers;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class ExecutionManager
    {
        public bool IsEngineBusy { get; private set; } = false;
        private Timer _newJobsCheckTimer;

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
                // If Jobs Queue is not Empty & No Job is being executed
                if(!JobsQueueManager.Instance.IsQueueEmpty() && !IsEngineBusy)
                {
                    SetEngineStatus(true);
                    ExecuteJob();
                    SetEngineStatus(false);
                }
            }
            catch (Exception)
            {
                var job = JobsQueueManager.Instance.DequeueJob();

                // Update Job Status (Fail)
                JobsAPIManager.UpdateJob(AuthAPIManager.Instance, job.Id.ToString(),
                    new List<Operation>()
                    {
                        new Operation(){ Op = "replace", Path = "/jobStatus", Value = nameof(JobStatus.Fail)},
                        new Operation(){ Op = "replace", Path = "/message", Value = "Job is failed"},
                        new Operation(){ Op = "replace", Path = "/isSuccessful", Value = false}
                    });

                SetEngineStatus(false);
            }
        }

        private void ExecuteJob()
        {
            // Peek Job
            var job = JobsQueueManager.Instance.PeekJob();

            // Update Job Status (InProgress)
            JobsAPIManager.UpdateJob(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/jobStatus", Value = nameof(JobStatus.InProgress)},
                    new Operation(){ Op = "replace", Path = "/message", Value = "Job is running"}
                });

            // Get Process Info
            var processInfo = ProcessesAPIManager.GetProcess(AuthAPIManager.Instance, job.ProcessId.ToString());

            // Download Process and Extract Files
            var mainScriptFilePath = ProcessManager.DownloadAndExtractProcess(processInfo);

            // Run Process
            RunProcess(processInfo.Name, mainScriptFilePath);

            // Update Job Status (Complete)
            JobsAPIManager.UpdateJob(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/jobStatus", Value = nameof(JobStatus.Complete)},
                    new Operation(){ Op = "replace", Path = "/message", Value = "Job is completed"},
                    new Operation(){ Op = "replace", Path = "/isSuccessful", Value = true}
                });

            // Dequeue the Job
            JobsQueueManager.Instance.DequeueJob();
        }
        private void RunProcess(string processName, string mainScriptFilePath)
        {
            var executionParams = GetExecutionParams(processName, mainScriptFilePath);
            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";
            
            // launch the Executor
            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, out procInfo);
        }
        private void SetEngineStatus(bool isBusy)
        {
            IsEngineBusy = isBusy;
            if (!IsEngineBusy)
                OnJobFinishedEvent(EventArgs.Empty);
        }

        protected virtual void OnJobFinishedEvent(EventArgs e)
        {
            JobFinishedEvent?.Invoke(this, e);
        }

        private string GetExecutionParams(string processName, string mainScriptFilePath)
        {
            var executionParams = new JobExecutionParams()
            {
                ProcessName = processName,
                MainFilePath = mainScriptFilePath,
                ProjectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath),
                ServerConnectionSettings = ConnectionSettingsManager.Instance.ConnectionSettings
            };

            return JsonSerializer.Serialize(executionParams);
        }
    }
}
