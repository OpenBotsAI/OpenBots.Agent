using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Client;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.HeartBeat;
using OpenBots.Service.Client.Manager.Hub;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Settings;
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

        private ConnectionSettingsManager _connectionSettingsManager;
        private AuthAPIManager _authAPIManager;
        public ExecutionManager ExecutionManager;
        public JobsPolling(AgentHeartBeatManager agentHeartBeatManager)
        {
            ExecutionManager = new ExecutionManager(agentHeartBeatManager);
        }

        private void Initialize(ConnectionSettingsManager connectionSettingsManager, AuthAPIManager authAPIManager)
        {
            _connectionSettingsManager = connectionSettingsManager;
            _authAPIManager = authAPIManager;

            _connectionSettingsManager.ConnectionSettingsUpdatedEvent += OnConnectionSettingsUpdate;
            _authAPIManager.ConfigurationUpdatedEvent += OnConfigurationUpdate;
        }

        private void UnInitialize()
        {
            if(_connectionSettingsManager != null)
                _connectionSettingsManager.ConnectionSettingsUpdatedEvent -= OnConnectionSettingsUpdate;
            if(_authAPIManager != null)
                _authAPIManager.ConfigurationUpdatedEvent -= OnConfigurationUpdate;
        }

        public void StartJobsPolling(ConnectionSettingsManager connectionSettingsManager, AuthAPIManager authAPIManager)
        {
            Initialize(connectionSettingsManager, authAPIManager);

            // Start Timed Polling
            StartJobsFetchTimer();

            // Start Long Polling
            StartHubManager();

            // Start Execution Manager to Run Job(s)
            StartExecutionManager();
        }

        
        public void StopJobsPolling()
        {
            UnInitialize();

            // Stop Timed Polling
            StopJobsFetchTimer();

            // Stop Long Polling
            StopHubManager();

            // Stop Execution Manager
            StopExecutionManager();
        }
        

        #region TimedPolling
        private void StartJobsFetchTimer()
        {
            if (_connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled)
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
            //FileLogger.Instance.LogEvent("Timed Polling", "Started Timed Polling");
        }
        private void StopJobsFetchTimer()
        {
            if (_newJobsFetchTimer != null)
            {
                _newJobsFetchTimer.Enabled = false;
                _newJobsFetchTimer.Elapsed -= JobsFetchTimer_Elapsed;

                // Log Event
                //FileLogger.Instance.LogEvent("Timed Polling", "Stopped Timed Polling");
            }
        }
        private void JobsFetchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Log Event
            //FileLogger.Instance.LogEvent("Timed Polling", "Attempt to fetch new job");
            FetchNewJobs();
        }

        #endregion TimedPolling

        #region LongPolling
        private void StartHubManager()
        {
            if (_jobsHubManager == null)
                _jobsHubManager = new HubManager(_connectionSettingsManager.ConnectionSettings);

            _jobsHubManager.JobNotificationReceived += OnNewJobAddedEvent;
            _jobsHubManager.Connect();

            // Log Event
            //FileLogger.Instance.LogEvent("Long Polling", "Started Long Polling");
        }

        private void OnNewJobAddedEvent(string agentId)
        {
            // Log Event
            //FileLogger.Instance.LogEvent("Long Polling", $"New job notification received for AgentId \"{agentId}\"");
            if (_connectionSettingsManager.ConnectionSettings.AgentId == agentId)
            {
                // Log Event
                //FileLogger.Instance.LogEvent("Job Fetch", $"Attempt to fetch new Job for AgentId \"{ConnectionSettingsManager.Instance.ConnectionSettings.AgentId}\"");

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
                //FileLogger.Instance.LogEvent("Long Polling", "Stopped Long Polling");
            }
        }

        #endregion LongPolling

        private void FetchNewJobs()
        {
            try
            {
                //Retrieve New Jobs for this Agent
                var apiResponse = JobsAPIManager.GetJob(
                    _authAPIManager,
                    _connectionSettingsManager.ConnectionSettings.AgentId);

                if (apiResponse.Data.AssignedJob != null)
                {
                    ExecutionManager.JobsQueueManager.EnqueueJob(apiResponse.Data.AssignedJob);

                    // Log Event
                    //FileLogger.Instance.LogEvent("Job Fetch", "Job fetched and queued for execution");
                }
            }
            catch (Exception ex)
            {
                // Log Event
                //FileLogger.Instance.LogEvent("Job Fetch", $"Error occurred while fetching new job; Error Message = {ex.ToString()}",
                //    LogEventLevel.Error);

                throw ex;
            }
        }

        private void StartExecutionManager()
        {
            ExecutionManager.JobFinishedEvent += OnJobFinished;
            ExecutionManager.StartNewJobsCheckTimer(_connectionSettingsManager, _authAPIManager);
        }
        private void StopExecutionManager()
        {
            ExecutionManager.JobFinishedEvent -= OnJobFinished;
            ExecutionManager.StopNewJobsCheckTimer();
        }

        private void OnJobFinished(object sender, EventArgs e)
        {
            FetchNewJobs();
        }

        private void OnConfigurationUpdate(object sender, Configuration configuration)
        {
            _authAPIManager.Configuration = configuration;
        }

        private void OnConnectionSettingsUpdate(object sender, ServerConnectionSettings connectionSettings)
        {
            _connectionSettingsManager.ConnectionSettings = connectionSettings;
        }
    }
}
