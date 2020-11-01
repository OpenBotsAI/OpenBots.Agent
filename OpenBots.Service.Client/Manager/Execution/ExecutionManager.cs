
using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
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
using System.Text;
using System.Timers;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class ExecutionManager
    {
        public bool IsEngineBusy { get; private set; } = false;
        private Timer _newJobsCheckTimer;
        private ProcessExecutionLog _executionLog;

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
            catch (Exception ex)
            {   
                // Log Event
                FileLogger.Instance.LogEvent("Job Execution", $"Error occurred while executing the job; ErrorMessage = {ex.ToString()}", 
                    LogEventLevel.Error);

                try
                {
                    var job = JobsQueueManager.Instance.DequeueJob();

                    // Update Process Execution Log (Execution Failure)
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
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to fetch Process Detail");

            // Get Process Info
            var process = ProcessesAPIManager.GetProcess(AuthAPIManager.Instance, job.ProcessId.ToString());

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to download/retrieve Process");

            // Download Process and Extract Files
            var mainScriptFilePath = ProcessManager.DownloadAndExtractProcess(process);

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to update Job Status (Pre-execution)");

            // Create Process Execution Log (Execution Started)
            _executionLog = ExecutionLogsAPIManager.CreateExecutionLog(AuthAPIManager.Instance, new ProcessExecutionLog(job.Id,
                job.ProcessId, job.AgentId, DateTime.Now, null, null, null, "Job has started processing"));

            // Update Job Status (InProgress)
            JobsAPIManager.UpdateJobStatus(AuthAPIManager.Instance, job.AgentId.ToString(), job.Id.ToString(),
                JobStatusType.InProgress, new JobErrorViewModel());

            // Update Job Start Time
            JobsAPIManager.UpdateJobPatch(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/startTime", Value = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")}
                });

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to execute process");

            //AgentViewModel agent = AgentsAPIManager.GetAgent(AuthAPIManager.Instance, job.AgentId.ToString());
            //Credential creds = CredentialsAPIManager.GetCredentials(AuthAPIManager.Instance, agent.CredentialId.ToString());

            // Run Process
            RunProcess(job, process, mainScriptFilePath);

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Process execution completed");

            // Log Event
            FileLogger.Instance.LogEvent("Job Execution", "Attempt to update Job Status (Post-execution)");

            // Update Job End Time
            JobsAPIManager.UpdateJobPatch(AuthAPIManager.Instance, job.Id.ToString(),
                new List<Operation>()
                {
                    new Operation(){ Op = "replace", Path = "/endTime", Value = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'")},
                });

            // Delete Process Files Directory
            Directory.Delete(Path.GetDirectoryName(mainScriptFilePath), true);

            // Update Process Execution Log (Execution Finished)
            _executionLog.CompletedOn = DateTime.Now;
            _executionLog.Status = "Job has finished processing";
            ExecutionLogsAPIManager.UpdateExecutionLog(AuthAPIManager.Instance, _executionLog);

            // Update Job Status (Completed)
            JobsAPIManager.UpdateJobStatus(AuthAPIManager.Instance, job.AgentId.ToString(), job.Id.ToString(),
                JobStatusType.Completed, new JobErrorViewModel());

            FileLogger.Instance.LogEvent("Job Execution", "Job status updated. Removing from queue.");

            // Dequeue the Job
            JobsQueueManager.Instance.DequeueJob();
        }
        private void RunProcess(Job job, Process process, string mainScriptFilePath)
        {
            try
            {
                var executionParams = GetExecutionParams(job, process, mainScriptFilePath);
                var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
                var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";

                // launch the Executor
                ProcessLauncher.PROCESS_INFORMATION procInfo;
                ProcessLauncher.LaunchProcess(cmdLine, out procInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        private string GetExecutionParams(Job job, Process process, string mainScriptFilePath)
        {
            var executionParams = new JobExecutionParams()
            {
                JobId = job.Id.ToString(),
                ProcessId = process.Id.ToString(),
                ProcessName = process.Name,
                MainFilePath = mainScriptFilePath,
                ProjectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath),
                ServerConnectionSettings = ConnectionSettingsManager.Instance.ConnectionSettings
            };
            var paramsJsonString = JsonConvert.SerializeObject(executionParams);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(paramsJsonString));
        }
    }
}
