using OpenBots.Agent.Core.Enums;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Settings;
using Serilog.Events;
using System;
using System.Timers;

namespace OpenBots.Service.Client.Manager.HeartBeat
{
    public class AgentHeartBeatManager
    {
        private Timer _heartbeatTimer;
        private AuthAPIManager _authAPIManager;
        private ConnectionSettingsManager _connectionSettingsManager;
        private FileLogger _fileLogger;
        public AgentHeartbeat Heartbeat { get; set; }

        public event EventHandler ServerConnectionLostEvent;

        public AgentHeartBeatManager()
        {
            Heartbeat = new AgentHeartbeat();
        }

        private void Initialize(ConnectionSettingsManager connectionSettingsManager, AuthAPIManager authAPIManager, FileLogger fileLogger)
        {
            _connectionSettingsManager = connectionSettingsManager;
            _authAPIManager = authAPIManager;
            _fileLogger = fileLogger;
            _authAPIManager.ConfigurationUpdatedEvent += OnConfigurationUpdate;
        }

        private void UnInitialize()
        {
            if (_authAPIManager != null)
                _authAPIManager.ConfigurationUpdatedEvent -= OnConfigurationUpdate;
        }

        public void StartHeartBeatTimer(ConnectionSettingsManager connectionSettingsManager, AuthAPIManager authAPIManager, FileLogger fileLogger)
        {
            Initialize(connectionSettingsManager, authAPIManager, fileLogger);

            if (_connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled)
            {
                //handle for reinitialization
                if (_heartbeatTimer != null)
                {
                    _heartbeatTimer.Elapsed -= Heartbeat_Elapsed;
                }

                //setup heartbeat to the server
                _heartbeatTimer = new Timer();
                _heartbeatTimer.Interval = 30000;
                _heartbeatTimer.Elapsed += Heartbeat_Elapsed;
                _heartbeatTimer.Enabled = true;

                Initialize(_connectionSettingsManager.ConnectionSettings.AgentId);
            }
        }

        public void StopHeartBeatTimer()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Enabled = false;
                _heartbeatTimer.Elapsed -= Heartbeat_Elapsed;

                UnInitialize();
            }
        }

        private void Heartbeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            int statusCode = 0;
            try
            {
                // Update LastReportedOn
                Heartbeat.LastReportedOn = DateTime.UtcNow;

                // Send HeartBeat to the Server
                statusCode = AgentsAPIManager.SendAgentHeartBeat(
                    _authAPIManager,
                    _connectionSettingsManager.ConnectionSettings.AgentId,
                    Heartbeat);

                if (statusCode != 201)
                {
                    _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                    _connectionSettingsManager.UpdateConnectionSettings(_connectionSettingsManager.ConnectionSettings);
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

        public void Initialize(string agentId)
        {
            Heartbeat.AgentId = new Guid(agentId);
            Heartbeat.LastReportedStatus = AgentStatus.Available.ToString();
            Heartbeat.LastReportedWork = string.Empty;
            Heartbeat.LastReportedMessage = string.Empty;
            Heartbeat.IsHealthy = true;
        }

        #region Event Handlers
        private void OnConfigurationUpdate(object sender, Configuration configuration)
        {
            _authAPIManager.Configuration = configuration;
        }
        #endregion
    }
}
