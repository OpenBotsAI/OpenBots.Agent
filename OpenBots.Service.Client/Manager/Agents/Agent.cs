using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.HelperMethods;
using OpenBots.Server.SDK.Model;
using OpenBots.Service.Client.Manager.Common;
using OpenBots.Service.Client.Manager.Execution;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Settings;
using Serilog.Events;
using System;
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

            try
            {
                // Authenticate Agent
                _authAPIManager.AuthMethods.Initialize(connectionSettings.ServerType,
                    connectionSettings.OrganizationName, connectionSettings.ServerURL, 
                    connectionSettings.AgentUsername, connectionSettings.AgentPassword, 
                    connectionSettings.AgentId ?? "", connectionSettings.UserName, connectionSettings.DNSHost);

                _authAPIManager.UserInfo = _authAPIManager.AuthMethods.GetUserInfo();

                // Call to Resolve (to get AgentId, AgentName, AgentGroup, HeartbeatInterval, JobLoggingInterval, SSLVerification)
                var resolveApiResponse = AgentMethods.ResolveAgent(_authAPIManager.UserInfo, new ResolveAgentViewModel(null, null, connectionSettings.MachineName, null));

                _connectionSettingsManager.ConnectionSettings.AgentId = resolveApiResponse.AgentId.ToString();
                _connectionSettingsManager.ConnectionSettings.ServerURL = _authAPIManager.UserInfo.ServerUrl;

                // API Call to Connect
                var connectAPIResponse = AgentMethods.ConnectAgent(_authAPIManager.UserInfo, _connectionSettingsManager.ConnectionSettings.AgentId,
                    new ConnectAgentViewModel{
                        MachineName = _connectionSettingsManager.ConnectionSettings.MachineName,
                        MacAddresses = _connectionSettingsManager.ConnectionSettings.MACAddress
                    });

                // Update Server Settings
                _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = true;
                _connectionSettingsManager.ConnectionSettings.AgentId = connectAPIResponse.AgentId.ToString();
                _connectionSettingsManager.ConnectionSettings.AgentName = connectAPIResponse.AgentName.ToString();

                // Update Logging URL for Cloud
                UpdateLoggingURL();

                if(resolveApiResponse.HeartbeatInterval != null)
                    _connectionSettingsManager.ConnectionSettings.HeartbeatInterval = (int)resolveApiResponse.HeartbeatInterval;
                
                if(resolveApiResponse.JobLoggingInterval != null)
                    _connectionSettingsManager.ConnectionSettings.JobsLoggingInterval = (int)resolveApiResponse.JobLoggingInterval;
                
                if(resolveApiResponse.VerifySslCertificate != null)
                    _connectionSettingsManager.ConnectionSettings.SSLCertificateVerification = (bool)resolveApiResponse.VerifySslCertificate;

                // Start Server Communication
                StartServerCommunication();

                // Send Response to Agent
                return new ServerResponse(_connectionSettingsManager.ConnectionSettings);
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
                AgentMethods.DisconnectAgent(_authAPIManager.UserInfo, _connectionSettingsManager.ConnectionSettings.AgentId,
                    new ConnectAgentViewModel
                    {
                        MachineName = _connectionSettingsManager.ConnectionSettings.MachineName,
                        MacAddresses = _connectionSettingsManager.ConnectionSettings.MACAddress
                    });

                // Update settings
                _connectionSettingsManager.ConnectionSettings.ServerConnectionEnabled = false;
                _connectionSettingsManager.ConnectionSettings.AgentId = string.Empty;
                _connectionSettingsManager.ConnectionSettings.AgentName = string.Empty;

                // Stop Server Communication
                StopServerCommunication();

                // Form Server Response
                return new ServerResponse(_connectionSettingsManager.ConnectionSettings);
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

        public ServerResponse GetAutomations(string automationEngine)
        {
            try
            {
                var serverSettings = _connectionSettingsManager.ConnectionSettings;

                string filter = $"automationEngine eq '{automationEngine}'";
                var apiResponse = AutomationMethods.GetAutomations(_authAPIManager.UserInfo, filter);
                var automationPackageNames = apiResponse.Items.Where(
                    a => !string.IsNullOrEmpty(a.OriginalPackageName) &&
                    a.OriginalPackageName.EndsWith(".nupkg")
                    ).Select(a => a.OriginalPackageName).ToList();
                return new ServerResponse(automationPackageNames);
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
                _authAPIManager.AuthMethods.Initialize(serverSettings.ServerType, 
                    serverSettings.OrganizationName, serverSettings.ServerURL, 
                    serverSettings.AgentUsername, serverSettings.AgentPassword, 
                    serverSettings.AgentId ?? "", serverSettings.UserName, serverSettings.DNSHost);

                var serverIP = _authAPIManager.AuthMethods.Ping();

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
        private void OnServerConnectionLost(object sender, EventArgs e)
        {
            StopServerCommunication();
        }
        #endregion

        #region Helper Methods
        // Update Logging URL for Cloud Only
        private void UpdateLoggingURL()
        {
            if (_connectionSettingsManager.ConnectionSettings.ServerType == OrchestratorType.Cloud.ToString() &&
                _connectionSettingsManager.ConnectionSettings.LogStorage == OrchestratorType.Cloud.ToString())
            {
                Uri baseUri = new Uri(_connectionSettingsManager.ConnectionSettings.ServerURL);
                _connectionSettingsManager.ConnectionSettings.LogUrl = new Uri(baseUri, _connectionSettingsManager.ConnectionSettings.LogUrl).ToString();
            }
        }
        #endregion

    }
}
