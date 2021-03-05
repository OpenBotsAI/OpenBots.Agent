using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Hub;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Settings;
using Serilog.Events;
using System;
using System.Timers;

namespace OpenBots.Service.Client.Manager.Execution
{
    public class JobsPolling
    {
        // Heartbeat Timer
        private Timer _heartbeatTimer;

        // Jobs Hub Manager (for Long Polling)
        private HubManager _jobsHubManager;

        private ConnectionSettingsManager _connectionSettingsManager;
        private AuthAPIManager _authAPIManager;
        private FileLogger _fileLogger;

        public HeartbeatViewModel Heartbeat { get; set; }
        public event EventHandler ServerConnectionLostEvent;
        public ExecutionManager ExecutionManager;
        public JobsPolling()
        {
        }

        private void Initialize(ConnectionSettingsManager connectionSettingsManager, AuthAPIManager authAPIManager, FileLogger fileLogger)
        {
            _connectionSettingsManager = connectionSettingsManager;
            _authAPIManager = authAPIManager;
            _fileLogger = fileLogger;

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

        public void StartJobsPolling(ConnectionSettingsManager connectionSettingsManager, AuthAPIManager authAPIManager, FileLogger fileLogger)
        {
            Initialize(connectionSettingsManager, authAPIManager, fileLogger);

            // Start Heartbeat Timer
            StartHeartbeatTimer();

            // Start Long Polling
            StartHubManager();

            // Start Execution Manager to Run Job(s)
            StartExecutionManager();
        }

        
        public void StopJobsPolling()
        {
            UnInitialize();

            // Stop Heartbeat Timer
            StopHeartbeatTimer();

            // Stop Long Polling
            StopHubManager();

            // Stop Execution Manager
            StopExecutionManager();
        }
        

        #region Heartbeat/TimedPolling
        private void StartHeartbeatTimer()
        {
            if (_connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled)
            {
                //handle for reinitialization
                if (_heartbeatTimer != null)
                {
                    _heartbeatTimer.Elapsed -= HeartbeatTimer_Elapsed;
                }

                //setup heartbeat to the server
                _heartbeatTimer = new Timer();
                _heartbeatTimer.Interval = (_connectionSettingsManager.ConnectionSettings.HeartbeatInterval * 1000);
                _heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
                _heartbeatTimer.Enabled = true;

                InitializeHeartbeat();
            }

            // Log Event
            _fileLogger.LogEvent("Heartbeat", "Started Heartbeat Timer");
        }
        private void StopHeartbeatTimer()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Enabled = false;
                _heartbeatTimer.Elapsed -= HeartbeatTimer_Elapsed;

                // Log Event
                _fileLogger.LogEvent("Heartbeat", "Stopped Heartbeat Timer");
            }
        }

        private void InitializeHeartbeat()
        {
            if(Heartbeat == null)
                Heartbeat = new HeartbeatViewModel();

            Heartbeat.LastReportedStatus = AgentStatus.Available.ToString();
            Heartbeat.LastReportedWork = string.Empty;
            Heartbeat.LastReportedMessage = string.Empty;
            Heartbeat.IsHealthy = true;
            Heartbeat.GetNextJob = true;
        }

        private void HeartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Log Event
            _fileLogger.LogEvent("Heartbeat", "Heartbeat Timer Elapsed");
            SendHeartbeat();
        }

        #endregion Heartbeat/TimedPolling

        #region LongPolling
        private void StartHubManager()
        {
            if (_jobsHubManager == null)
                _jobsHubManager = new HubManager(_connectionSettingsManager.ConnectionSettings);

            _jobsHubManager.JobNotificationReceived += OnNewJobAddedEvent;
            _jobsHubManager.Connect();

            // Log Event
            _fileLogger.LogEvent("Long Polling", "Started Long Polling");
        }

        private void OnNewJobAddedEvent(string agentId)
        {
            // Log Event
            _fileLogger.LogEvent("Long Polling", $"New job notification received for AgentId \"{agentId}\"");
            if (_connectionSettingsManager.ConnectionSettings.AgentId == agentId)
            {
                // Log Event
                _fileLogger.LogEvent("Job Fetch", $"Attempt to fetch new Job for AgentId \"{_connectionSettingsManager.ConnectionSettings.AgentId}\"");

                SendHeartbeat();
            }
        }

        private void StopHubManager()
        {
            if (_jobsHubManager != null)
            {
                _jobsHubManager.Disconnect();
                _jobsHubManager.JobNotificationReceived -= OnNewJobAddedEvent;

                // Log Event
                _fileLogger.LogEvent("Long Polling", "Stopped Long Polling");
            }
        }

        #endregion LongPolling

        private void SendHeartbeat()
        {
            int statusCode = 0;
            try
            {
                // Update LastReportedOn
                Heartbeat.LastReportedOn = DateTime.UtcNow;

                // Send HeartBeat to the Server
                var apiResponse = AgentsAPIManager.SendAgentHeartBeat(
                    _authAPIManager,
                    _connectionSettingsManager.ConnectionSettings.AgentId,
                    Heartbeat);

                statusCode = apiResponse.StatusCode;
                if (statusCode != 200)
                {
                    _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                    _connectionSettingsManager.UpdateConnectionSettings(_connectionSettingsManager.ConnectionSettings);
                }
                else if (apiResponse.Data?.AssignedJob != null)
                {
                    ExecutionManager.JobsQueueManager.EnqueueJob(apiResponse.Data.AssignedJob);

                    // Log Event
                    _fileLogger.LogEvent("Job Fetch", "Job fetched and queued for execution");
                }
            }
            catch (Exception ex)
            {
                _fileLogger.LogEvent("HeartBeat", $"Status Code: {statusCode} || Exception: {ex.ToString()}", LogEventLevel.Error);
                _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                _connectionSettingsManager.UpdateConnectionSettings(_connectionSettingsManager.ConnectionSettings);

                // Invoke event to Stop Server Communication
                ServerConnectionLostEvent?.Invoke(this, EventArgs.Empty);
            }
        }

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
                    _fileLogger.LogEvent("Job Fetch", "Job fetched and queued for execution");
                }
            }
            catch (Exception ex)
            {
                // Log Event
                _fileLogger.LogEvent("Job Fetch", $"Error occurred while fetching new job; Error Message = {ex.ToString()}",
                    LogEventLevel.Error);

                throw ex;
            }
        }

        private void StartExecutionManager()
        {
            if (ExecutionManager == null)
                ExecutionManager = new ExecutionManager(Heartbeat);

            ExecutionManager.JobFinishedEvent += OnJobFinished;
            ExecutionManager.StartNewJobsCheckTimer(_connectionSettingsManager, _authAPIManager, _fileLogger);
        }
        private void StopExecutionManager()
        {
            if(ExecutionManager != null)
            {
                ExecutionManager.JobFinishedEvent -= OnJobFinished;
                ExecutionManager.StopNewJobsCheckTimer();
            }
        }

        private void OnJobFinished(object sender, EventArgs e)
        {
            SendHeartbeat();
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
