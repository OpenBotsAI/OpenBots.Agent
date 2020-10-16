using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using OpenBots.Service.Client.Manager.Execution;
using OpenBots.Service.Client.Manager;

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
            try
            {
                int statusCode = AgentsAPIManager.SendAgentHeartBeat(
                    AuthAPIManager.Instance,
                    ConnectionSettingsManager.Instance.ConnectionSettings.AgentId,
                    new HeartbeatViewModel(DateTime.Now, "", "", "", true));

                if (statusCode != 200)
                    ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled = false;
            }
            catch (Exception)
            {
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
            _jobsPolling.StopJobsPolling();
        }

        #endregion JobsPolling

        #region ServerConnection
        public ServerResponse Connect(ServerConnectionSettings connectionSettings)
        {
            ConnectionSettingsManager.Instance.ConnectionSettings = connectionSettings;

            // Initialize AuthAPIManager
            AuthAPIManager.Instance.Initialize(ConnectionSettingsManager.Instance.ConnectionSettings);
            try
            {
                // Authenticate Agent
                AuthenticateAgent();

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

                var errorMessage = ex.GetType().GetProperty("ErrorContent").GetValue(ex, null)?.ToString();
                errorMessage = errorMessage ?? ex.GetType().GetProperty("Message").GetValue(ex, null)?.ToString();
                // Send Response to Agent
                return new ServerResponse(null,
                    ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString(),
                    errorMessage);
            }
        }
        public ServerResponse Disconnect(ServerConnectionSettings connectionSettings)
        {
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

                // Form Server Response
                return new ServerResponse(ConnectionSettingsManager.Instance.ConnectionSettings, apiResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                var errorMessage = ex.GetType().GetProperty("ErrorContent").GetValue(ex, null)?.ToString();
                errorMessage = errorMessage ?? ex.GetType().GetProperty("Message").GetValue(ex, null)?.ToString();

                // Form Server Response
                return new ServerResponse(null,
                    ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString(),
                    errorMessage);
            }
        }
        private void AuthenticateAgent()
        {
            // API Call to Get Token (Login)
            try
            {
                AuthAPIManager.Instance.GetToken();
            }
            catch (Exception ex)
            {
                // If Unauthorized Request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Create Agent User
                    AuthAPIManager.Instance.RegisterAgentUser();

                    // Get Token after successful SignUp
                    AuthAPIManager.Instance.GetToken();

                }
                else
                    throw ex;
            }

            // Create Agent if doesn't exist
            try
            {
                if (!AgentsAPIManager.FindAgent(AuthAPIManager.Instance, $"name eq '{ConnectionSettingsManager.Instance.ConnectionSettings.AgentUsername}'"))
                    AgentsAPIManager.CreateAgent(AuthAPIManager.Instance, ConnectionSettingsManager.Instance.ConnectionSettings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ServerConnection

    }
}
