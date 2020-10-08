
using OpenBots.Agent.Core.Enums;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace OpenBots.Service.Client.Executor
{
    public class ExecutionManager
    {
        public bool IsEngineBusy { get; private set; } = false;
        private Timer _newJobsCheckTimer;

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
            _newJobsCheckTimer = new Timer();
            _newJobsCheckTimer.Interval = 5000;
        }

        public void StartNewJobsCheckTimer()
        {
            //handle for reinitialization
            if (_newJobsCheckTimer != null)
            {
                _newJobsCheckTimer.Elapsed -= NewJobsCheckTimer_Elapsed;
            }

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

        private void NewJobsCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // If Jobs Queue is not Empty
                if(!JobsQueueManager.Instance.IsQueueEmpty())
                {
                    SetEngineStatus(true);
                    StartAutomation();
                }
            }
            catch (Exception ex)
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
            }
            finally
            {
                SetEngineStatus(false);
            }
        }

        private void StartAutomation()
        {
            // Dequeue Job
            var job = JobsQueueManager.Instance.PeekJob();

            // Update Job Status (InProgress)
            JobsAPIManager.UpdateJob(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/jobStatus", Value = nameof(JobStatus.InProgress)},
                    new Operation(){ Op = "replace", Path = "/message", Value = "Job is running"}
                });

            // Download Process and Extract Files
            var mainScriptFilePath = ProcessManager.DownloadAndExtractProcess(job.ProcessId.ToString());

            // Run Process
            RunJob(mainScriptFilePath);

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

        private void SetEngineStatus(bool isBusy)
        {
            IsEngineBusy = isBusy;
            if (IsEngineBusy)
                StopNewJobsCheckTimer();
            else
                StartNewJobsCheckTimer();
        }

        private void RunJob(string mainFilePath)
        {
            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{mainFilePath}\"";
            // launch the application
            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, out procInfo);
        }
    }
}
