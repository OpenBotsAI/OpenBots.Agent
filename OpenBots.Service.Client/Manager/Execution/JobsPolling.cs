using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Hub;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Server;
using Serilog.Events;
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

            // Log Event
            FileLogger.Instance.LogEvent("Timed Polling", "Started Timed Polling");
        }
        private void StopJobsFetchTimer()
        {
            if (_newJobsFetchTimer != null)
            {
                _newJobsFetchTimer.Enabled = false;
                _newJobsFetchTimer.Elapsed -= JobsFetchTimer_Elapsed;

                // Log Event
                FileLogger.Instance.LogEvent("Timed Polling", "Stopped Timed Polling");
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

            // Log Event
            FileLogger.Instance.LogEvent("Long Polling", "Started Long Polling");
        }

        private void OnNewJobAddedEvent(string agentId)
        {
            // Log Event
            FileLogger.Instance.LogEvent("Long Polling", $"New job notification received for AgentId \"{agentId}\"");
            if (ConnectionSettingsManager.Instance.ConnectionSettings.AgentId == agentId)
            {
                // Log Event
                FileLogger.Instance.LogEvent("Job Fetch", $"Attempt to fetch new Job for AgentId \"{ConnectionSettingsManager.Instance.ConnectionSettings.AgentId}\"");

                FetchNewJobs();
            }
        }

        private void StopHubManager()
        {
            if (_jobsHubManager != null)
            {
                _jobsHubManager.Disconnect();
                _jobsHubManager.JobNotificationReceived -= OnNewJobAddedEvent;

                // Log Event
                FileLogger.Instance.LogEvent("Long Polling", "Stopped Long Polling");
            }
        }

        #endregion LongPolling

        private void FetchNewJobs()
        {
            try
            {
                //Retrieve New Jobs for this Agent
                var apiResponse = JobsAPIManager.GetJob(
                    AuthAPIManager.Instance,
                    ConnectionSettingsManager.Instance.ConnectionSettings.AgentId);

                if (apiResponse.Data.AssignedJob != null)
                {
                    JobsQueueManager.Instance.EnqueueJob(apiResponse.Data.AssignedJob);

                    // Log Event
                    FileLogger.Instance.LogEvent("Job Fetch", "Job fetched and queued for execution");
                }
            }
            catch (Exception ex)
            {
                // Log Event
                FileLogger.Instance.LogEvent("Job Fetch", $"Error occurred while fetching new job; Error Message = {ex.ToString()}",
                    LogEventLevel.Error);

                throw ex;
            }
        }

        private void OnJobFinished(object sender, EventArgs e)
        {
            FetchNewJobs();
        }
    }
}
