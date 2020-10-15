using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Hub;
using OpenBots.Service.Client.Server;
using System;
using System.Timers;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class JobsPolling
    {
        // Jobs Fetch Timer (for Timed Polling)
        private Timer _newJobsFetchTimer;

        // Jobs Hub Manager (for Long Polling)
        private HubManager _jobsHubManager;

        public JobsPolling()
        {
        }

        public void StartJobsPolling()
        {
            // Start Timed Polling
            StartJobsFetchTimer();

            // Start Long Polling
            StartHubManager();

            // Start Execution Manager to Run Job(s)
            ExecutionManager.Instance.JobFinishedEvent += OnJobFinished;
            ExecutionManager.Instance.StartNewJobsCheckTimer();
        }
        public void StopJobsPolling()
        {
            // Stop Timed Polling
            StopJobsFetchTimer();

            // Stop Long Polling
            StopHubManager();

            // Stop Execution Manager
            ExecutionManager.Instance.JobFinishedEvent -= OnJobFinished;
            ExecutionManager.Instance.StopNewJobsCheckTimer();
        }

        #region TimedPolling
        private void StartJobsFetchTimer()
        {
            if (ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled)
            {
                //handle for reinitialization
                if (_newJobsFetchTimer != null)
                {
                    _newJobsFetchTimer.Elapsed -= JobsFetchTimer_Elapsed;
                }

                //setup heartbeat to the server
                _newJobsFetchTimer = new Timer();
                _newJobsFetchTimer.Interval = 300000;
                _newJobsFetchTimer.Elapsed += JobsFetchTimer_Elapsed;
                _newJobsFetchTimer.Enabled = true;
            }
        }
        private void StopJobsFetchTimer()
        {
            if (_newJobsFetchTimer != null)
            {
                _newJobsFetchTimer.Enabled = false;
                _newJobsFetchTimer.Elapsed -= JobsFetchTimer_Elapsed;
            }
        }
        private void JobsFetchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            FetchNewJobs();
        }

        #endregion TimedPolling

        #region LongPolling
        private void StartHubManager()
        {
            if (_jobsHubManager == null)
                _jobsHubManager = new HubManager();

            _jobsHubManager.JobNotificationReceived += OnNewJobAddedEvent;
            _jobsHubManager.Connect();
        }

        private void OnNewJobAddedEvent(string agentId)
        {
            if(ConnectionSettingsManager.Instance.ConnectionSettings.AgentId == agentId)
                FetchNewJobs();
        }

        private void StopHubManager()
        {
            if (_jobsHubManager != null)
            {
                _jobsHubManager.Disconnect();
                _jobsHubManager.JobNotificationReceived -= OnNewJobAddedEvent;
            }
        }

        #endregion LongPolling

        private void FetchNewJobs()
        {
            try
            {
                //Retrieve New Jobs for this Agent
                var apiResponse = JobsAPIManager.GetJobs(
                    AuthAPIManager.Instance,
                    $"agentId eq guid'{ConnectionSettingsManager.Instance.ConnectionSettings.AgentId}' and jobStatus eq 'New'");

                if (apiResponse.Data.Items.Count != 0)
                    foreach (var job in apiResponse.Data.Items)
                        JobsQueueManager.Instance.EnqueueJob(job);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OnJobFinished(object sender, EventArgs e)
        {
            FetchNewJobs();
        }
    }
}
