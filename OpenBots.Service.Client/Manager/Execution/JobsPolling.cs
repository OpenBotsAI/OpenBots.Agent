using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Hub;
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

        private ServerConnectionSettings _serverSettings;
        public JobsPolling(ServerConnectionSettings ServerSettings)
        {
            _serverSettings = ServerSettings;
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
            if (_serverSettings.ServerConnectionEnabled)
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
                _jobsHubManager = new HubManager(_serverSettings.ServerURL);

            _jobsHubManager.JobNotificationReceived += OnNewJobAddedEvent;
            _jobsHubManager.NewJobNotificationReceived += OnNewJobCreatedEvent;
            _jobsHubManager.Connect();
        }

        private void OnNewJobCreatedEvent(string obj)
        {
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
        private void OnNewJobAddedEvent(Tuple<Guid, Guid, Guid> tuple)
        {
            if(tuple.Item2.ToString() == _serverSettings.AgentId)
                FetchNewJobs();
        }

        #endregion LongPolling

        private void FetchNewJobs()
        {
            try
            {
                //Retrieve New Jobs for this Agent
                var apiResponse = JobsAPIManager.GetJobs(
                    AuthAPIManager.Instance,
                    $"agentId eq guid'{_serverSettings.AgentId}' and jobStatus eq 'New'");

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
