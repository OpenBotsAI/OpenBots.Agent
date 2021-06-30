using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Server.SDK.HelperMethods;
using OpenBots.Server.SDK.Model;
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
        private AuthMethods _authMethods;

        private ConnectionSettingsManager _connectionSettingsManager;
        private FileLogger _fileLogger;

        public HeartbeatViewModel Heartbeat { get; set; }
        public event EventHandler ServerConnectionLostEvent;
        public ExecutionManager ExecutionManager;
        public JobsPolling()
        {
            InitializeHeartbeat();
            ExecutionManager = new ExecutionManager(Heartbeat);
        }

        private void Initialize(ConnectionSettingsManager connectionSettingsManager, FileLogger fileLogger)
        {
            
            _connectionSettingsManager = connectionSettingsManager;
            _fileLogger = fileLogger;

            _connectionSettingsManager.ConnectionSettingsUpdatedEvent += OnConnectionSettingsUpdate;

            _authMethods = new AuthMethods(
                    _connectionSettingsManager.ConnectionSettings.ServerType,
                    _connectionSettingsManager.ConnectionSettings.OrganizationName,
                    _connectionSettingsManager.ConnectionSettings.ServerURL,
                    _connectionSettingsManager.ConnectionSettings.AgentUsername,
                    _connectionSettingsManager.ConnectionSettings.AgentPassword,
                    _connectionSettingsManager.ConnectionSettings.UserName,
                    _connectionSettingsManager.ConnectionSettings.DNSHost);
        }

        private void UnInitialize()
        {
            if (_connectionSettingsManager != null)
                _connectionSettingsManager.ConnectionSettingsUpdatedEvent -= OnConnectionSettingsUpdate;
        }

        public void StartJobsPolling(ConnectionSettingsManager connectionSettingsManager, FileLogger fileLogger)
        {
            Initialize(connectionSettingsManager, fileLogger);

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
            if (Heartbeat == null)
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

                // Authenticate Agent
                var userInfo = _authMethods.GetUserInfo();

                // Send HeartBeat to the Server
                var apiResponse = AgentMethods.SendAgentHeartBeat(
                    userInfo,
                    _connectionSettingsManager.ConnectionSettings.AgentId,
                    Heartbeat);

                if (apiResponse == null)
                {
                    _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                    _connectionSettingsManager.UpdateConnectionSettings(_connectionSettingsManager.ConnectionSettings);
                }
                else if (apiResponse.AssignedJob != null)
                {
                    ExecutionManager.JobsQueueManager.EnqueueJob(apiResponse.AssignedJob);

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

        private void StartExecutionManager()
        {
            if (ExecutionManager != null)
            {
                ExecutionManager.JobStartedEvent += OnJobStarted;
                ExecutionManager.JobFinishedEvent += OnJobFinished;
                ExecutionManager.StartNewJobsCheckTimer(_connectionSettingsManager, _fileLogger);
            }
        }

        private void StopExecutionManager()
        {
            if (ExecutionManager != null)
            {
                ExecutionManager.JobFinishedEvent -= OnJobFinished;
                ExecutionManager.StopNewJobsCheckTimer();
            }
        }

        private void OnJobStarted(object sender, EventArgs e)
        {
            StopHeartbeatTimer();
        }

        private void OnJobFinished(object sender, EventArgs e)
        {
            StartHeartbeatTimer();
            SendHeartbeat();
        }

        private void OnConnectionSettingsUpdate(object sender, ServerConnectionSettings connectionSettings)
        {
            _connectionSettingsManager.ConnectionSettings = connectionSettings;
        }
    }
}
