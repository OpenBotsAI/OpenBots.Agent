
using OpenBots.Service.Client.Manager;
using System;
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
                // ---- API Call to Update Job Status
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

            // Download Process and Extract Files
            var mainScriptFilePath = ProcessManager.DownloadAndExtractProcess(job.ProcessId.ToString());

            // Run Process
            RunJob(mainScriptFilePath);

            // Update Job Status (Complete)
            // ---- API Call to Update Job Status

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
