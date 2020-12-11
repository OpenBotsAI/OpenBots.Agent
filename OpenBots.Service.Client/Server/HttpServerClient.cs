using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using OpenBots.Service.Client.Manager.Execution;
using OpenBots.Service.Client.Manager;
using System.Security.Authentication;
using OpenBots.Service.API.Client;
using OpenBots.Service.Client.Manager.Logs;
using Serilog.Events;

namespace OpenBots.Service.Client.Server
{
    public class HttpServerClient
    {
        private Timer _heartbeatTimer;
        private JobsPolling _jobsPolling;

        //public ServerConnectionSettings ServerSettings { get; set; }
        public static HttpServerClient Instance
        {
            get
            {
                if (instance == null)
                    instance = new HttpServerClient();

                return instance;
            }
        }
        private static HttpServerClient instance;

        private HttpServerClient()
        {
        }

        public void Initialize()
        {
            //Initialize Connection Settings
            ConnectionSettingsManager.Instance.Initialize();
        }
        public void UnInitialize()
        {
            StopHeartBeatTimer();
            StopJobPolling();
        }

        public Boolean IsConnected()
        {
            return ConnectionSettingsManager.Instance.ConnectionSettings?.ServerConnectionEnabled ?? false;
        }

        #region HeartBeat
        private void StartHeartBeatTimer()
        {
            if (ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled)
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
            }
        }

        private void StopHeartBeatTimer()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Enabled = false;
                _heartbeatTimer.Elapsed -= Heartbeat_Elapsed;
            }
        }

        private void Heartbeat_Elapsed(object sender, ElapsedEventArgs e)
        {
            int statusCode = 0;
            try
            {
                statusCode = AgentsAPIManager.SendAgentHeartBeat(
                    AuthAPIManager.Instance,
                    ConnectionSettingsManager.Instance.ConnectionSettings.AgentId,
                    new AgentHeartbeat(null, null, null, null, null, null, null, null, null, null, DateTime.Now, "", "", "", true));

                if (statusCode != 201)
                    ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled = false;
            }
            catch (Exception ex)
            {
                FileLogger.Instance.LogEvent("HeartBeat", $"Status Code: {statusCode} || Exception: {ex.ToString()}", LogEventLevel.Error);
                ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled = false;
            }
        }
        #endregion HeartBeat

        #region JobsPolling
        private void StartJobPolling()
        {
            _jobsPolling = new JobsPolling();
            _jobsPolling.StartJobsPolling();
        }
        private void StopJobPolling()
        {
            if (_jobsPolling != null)
                _jobsPolling.StopJobsPolling();
        }

        #endregion JobsPolling

        #region ServerConnection
        public ServerResponse Connect(ServerConnectionSettings connectionSettings)
        {
            // Initialize File Logger for Debug Purpose
            FileLogger.Instance.Initialize(new EnvironmentSettings().GetEnvironmentVariable());

            // Log Event
            FileLogger.Instance.LogEvent("Connect", "Attempt to connect to the Server");

            ConnectionSettingsManager.Instance.ConnectionSettings = connectionSettings;

            // Initialize AuthAPIManager
            AuthAPIManager.Instance.Initialize(ConnectionSettingsManager.Instance.ConnectionSettings);
            try
            {
                // Authenticate Agent
                AuthAPIManager.Instance.GetToken();

                // API Call to Connect
                var connectAPIResponse = AgentsAPIManager.ConnectAgent(AuthAPIManager.Instance, ConnectionSettingsManager.Instance.ConnectionSettings);

                // Update Server Settings
                ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled = true;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentId = connectAPIResponse.Data.AgentId.ToString();
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentName = connectAPIResponse.Data.AgentName.ToString();

                // On Successful Connection with Server
                StartHeartBeatTimer();
                StartJobPolling();

                // Send Response to Agent
                return new ServerResponse(ConnectionSettingsManager.Instance.ConnectionSettings, connectAPIResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                // Update Server Settings
                ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled = false;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentId = string.Empty;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentName = string.Empty;

                string errorMessage;
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;

                if (errorCode == "401")
                    errorMessage = "Authentication Error - \"Agent is not found for given credentials\"";
                else
                    errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Log Event (Error)
                FileLogger.Instance.LogEvent("Connect", $"Error occurred while connecting to the Server; " +
                    $"Error Code = {errorCode}; Error Message = {errorMessage}", LogEventLevel.Error);

                // Send Response to Agent
                return new ServerResponse(null, errorCode, errorMessage);
            }
        }
        public ServerResponse Disconnect(ServerConnectionSettings connectionSettings)
        {
            // Log Event
            FileLogger.Instance.LogEvent("Disconnect", "Attempt to disconnect from the Server");

            try
            {
                // API Call to Disconnect
                var apiResponse = AgentsAPIManager.DisconnectAgent(AuthAPIManager.Instance, ConnectionSettingsManager.Instance.ConnectionSettings);

                // Update settings
                //ServerSettings = connectionSettings;
                ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled = false;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentId = string.Empty;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentName = string.Empty;

                // After Disconnecting from Server
                StopHeartBeatTimer();
                StopJobPolling();

                // UnInitialize Configuration
                AuthAPIManager.Instance.UnInitialize();

                // Form Server Response
                return new ServerResponse(ConnectionSettingsManager.Instance.ConnectionSettings, apiResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;
                var errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Log Event (Error)
                FileLogger.Instance.LogEvent("Disconnect", $"Error occurred while disconnecting from the Server; " +
                    $"Error Code = {errorCode}; Error Message = {errorMessage}", LogEventLevel.Error);

                // Form Server Response
                return new ServerResponse(null, errorCode, errorMessage);
            }
        }

        #endregion ServerConnection
    }
}
