using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Client;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Execution;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Settings;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBots.Service.Client.Manager.Agents
{
    public class Agent
    {
        private ConnectionSettingsManager _connectionSettingsManager;
        private AuthAPIManager _authAPIManager;
        private JobsPolling _jobsPolling;
        private AttendedExecutionManager _attendedExecutionManager;
        private FileLogger _fileLogger;

        public Agent()
        {
            _connectionSettingsManager = new ConnectionSettingsManager();
            _authAPIManager = new AuthAPIManager();
            _jobsPolling = new JobsPolling();
            _attendedExecutionManager = new AttendedExecutionManager(_jobsPolling.ExecutionManager, _authAPIManager);
            _fileLogger = new FileLogger();

            _connectionSettingsManager.ConnectionSettingsUpdatedEvent += OnConnectionSettingsUpdate;
            _authAPIManager.ConfigurationUpdatedEvent += OnConfigurationUpdate;
            _jobsPolling.ServerConnectionLostEvent += OnServerConnectionLost;
        }


        #region Agent Requests
        public ServerResponse Connect(ServerConnectionSettings connectionSettings)
        {
            // Initialize File Logger for Debug Purpose
            _fileLogger.Initialize(new EnvironmentSettings().GetEnvironmentVariablePath(connectionSettings.DNSHost, connectionSettings.UserName));

            // Log Event
            _fileLogger.LogEvent("Connect", "Attempt to connect to the Server");

            _connectionSettingsManager.ConnectionSettings = connectionSettings;

            // Initialize AuthAPIManager
            _authAPIManager.Initialize(_connectionSettingsManager.ConnectionSettings);
            try
            {
                // Authenticate Agent
                _authAPIManager.GetToken();

                // API Call to Connect
                var connectAPIResponse = AgentsAPIManager.ConnectAgent(_authAPIManager, 
                    _connectionSettingsManager.ConnectionSettings = _authAPIManager.ConnectionSettings);

                // Update Server Settings
                _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = true;
                _connectionSettingsManager.ConnectionSettings.AgentId = connectAPIResponse.Data.AgentId.ToString();
                _connectionSettingsManager.ConnectionSettings.AgentName = connectAPIResponse.Data.AgentName.ToString();

                // Start Server Communication
                StartServerCommunication();

                // Send Response to Agent
                return new ServerResponse(_connectionSettingsManager.ConnectionSettings, connectAPIResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                // Update Server Settings
                _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                _connectionSettingsManager.ConnectionSettings.AgentId = string.Empty;
                _connectionSettingsManager.ConnectionSettings.AgentName = string.Empty;

                string errorMessage;
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;

                errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Log Event (Error)
                _fileLogger.LogEvent("Connect", $"Error occurred while connecting to the Server; " +
                    $"Error Code = {errorCode}; Error Message = {errorMessage}", LogEventLevel.Error);

                // Send Response to Agent
                return new ServerResponse(null, errorCode, errorMessage);
            }
        }
        public ServerResponse Disconnect(ServerConnectionSettings connectionSettings)
        {
            // Log Event
            _fileLogger.LogEvent("Disconnect", "Attempt to disconnect from the Server");

            try
            {
                // API Call to Disconnect
                var apiResponse = AgentsAPIManager.DisconnectAgent(_authAPIManager, _connectionSettingsManager.ConnectionSettings);

                // Update settings
                _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                _connectionSettingsManager.ConnectionSettings.AgentId = string.Empty;
                _connectionSettingsManager.ConnectionSettings.AgentName = string.Empty;

                // Stop Server Communication
                StopServerCommunication();

                // UnInitialize Configuration
                _authAPIManager.UnInitialize();

                // Form Server Response
                return new ServerResponse(_connectionSettingsManager.ConnectionSettings, apiResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;
                var errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Log Event (Error)
                _fileLogger.LogEvent("Disconnect", $"Error occurred while disconnecting from the Server; " +
                    $"Error Code = {errorCode}; Error Message = {errorMessage}", LogEventLevel.Error);

                // Form Server Response
                return new ServerResponse(null, errorCode, errorMessage);
            }
        }

        public ServerResponse GetAutomations()
        {
            try
            {
                var apiResponse = AutomationsAPIManager.GetAutomations(_authAPIManager);
                var automationPackageNames = apiResponse.Data.Items.Where(
                    a => !string.IsNullOrEmpty(a.OriginalPackageName) &&
                    a.OriginalPackageName.EndsWith(".nupkg") &&
                    a.AutomationEngine.Equals("OpenBots")
                    ).Select(a => a.OriginalPackageName).ToList();
                return new ServerResponse(automationPackageNames, apiResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;
                var errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Log Event (Error)
                _fileLogger.LogEvent("Get Automations", $"Error occurred while getting automations from the Server; " +
                    $"Error Code = {errorCode}; Error Message = {errorMessage}", LogEventLevel.Error);

                // Form Server Response
                return new ServerResponse(null, errorCode, errorMessage);
            }

        }
        public ServerConnectionSettings GetConnectionSettings()
        {
            if (IsConnectedToServer())
                return _connectionSettingsManager.ConnectionSettings;
            else
                return null;
        }
        public bool IsConnectedToServer()
        {
            return _connectionSettingsManager?.ConnectionSettings?.ServerConnectionEnabled ?? false;
        }
        public bool IsEngineBusy()
        {
            return _jobsPolling.ExecutionManager?.IsEngineBusy ?? false;
        }
        public ServerResponse PingServer(ServerConnectionSettings serverSettings)
        {
            try
            {
                _authAPIManager.Initialize(serverSettings);
                var serverIP = _authAPIManager.Ping();
                _authAPIManager.UnInitialize();

                return new ServerResponse(serverIP);
            }
            catch (Exception ex)
            {
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;
                var errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Send Response to Agent
                return new ServerResponse(null, errorCode, errorMessage);
            }
        }

        public bool ExecuteAttendedTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            return _attendedExecutionManager.ExecuteTask(projectPackage, settings, isServerAutomation);
        }

        #endregion

        #region Server Communication Handling
        public void StartServerCommunication()
        {
            // Start Jobs Polling
            _jobsPolling.StartJobsPolling(_connectionSettingsManager, _authAPIManager, _fileLogger);
        }
        public void StopServerCommunication()
        {
            // Stop Jobs Polling
            _jobsPolling.StopJobsPolling();
        }
        #endregion

        #region Event Handlers
        private void OnConnectionSettingsUpdate(object sender, ServerConnectionSettings connectionSettings)
        {
            _connectionSettingsManager.ConnectionSettings = connectionSettings;
        }
        private void OnConfigurationUpdate(object sender, Configuration configuration)
        {
            _authAPIManager.Configuration = configuration;
        }
        private void OnServerConnectionLost(object sender, EventArgs e)
        {
            StopServerCommunication();
        }
        #endregion

    }
}
