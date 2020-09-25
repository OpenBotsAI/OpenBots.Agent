
using System;
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
                    RunJob();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                SetEngineStatus(false);
            }
        }

        private void RunJob()
        {
            // Dequeue Job
            var job = JobsQueueManager.Instance.DequeueJob();

        }

        private void SetEngineStatus(bool isBusy)
        {
            IsEngineBusy = isBusy;
            if (IsEngineBusy)
                StopNewJobsCheckTimer();
            else
                StartNewJobsCheckTimer();
        }
    }
}
