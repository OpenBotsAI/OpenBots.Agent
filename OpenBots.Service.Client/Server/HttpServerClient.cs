using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using OpenBots.Service.Client.Manager.Execution;

namespace OpenBots.Service.Client.Server
{
    public class HttpServerClient
    {
        private Timer _heartbeatTimer;
        private JobsPolling _jobsPolling;

        public ServerConnectionSettings ServerSettings { get; set; }
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
            ServerSettings = new ServerConnectionSettings();
        }
        public void UnInitialize()
        {
            StopHeartBeatTimer();
            StopJobPolling();
        }

        public Boolean IsConnected()
        {
            return ServerSettings?.ServerConnectionEnabled ?? false;
        }

        #region HeartBeat
        private void StartHeartBeatTimer()
        {
            if (ServerSettings.ServerConnectionEnabled)
            {
                //handle for reinitialization
                if (_heartbeatTimer != null)
                {
                    _heartbeatTimer.Elapsed -= Heartbeat_Elapsed;
                }

                //setup heartbeat to the server
                _heartbeatTimer = new Timer();
                _heartbeatTimer.Interval = 5000;
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
                    ServerSettings.AgentId,
                    new List<Operation>()
                    {
                            new Operation(){ Op = "replace", Path = "/lastReportedOn", Value = DateTime.Now.ToString("s")},
                            new Operation(){ Op = "replace", Path = "/lastReportedStatus", Value = "SomeStatus"},
                            new Operation(){ Op = "replace", Path = "/lastReportedWork", Value = "SomeWork"},
                            new Operation(){ Op = "replace", Path = "/lastReportedMessage", Value = "SomeMessage"},
                            new Operation(){ Op = "replace", Path = "/isHealthy", Value = true}
                    });

                if (statusCode != 200)
                    ServerSettings.ServerConnectionEnabled = false;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion HeartBeat

        #region JobsPolling
        private void StartJobPolling()
        {
            _jobsPolling = new JobsPolling(ServerSettings);
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
            ServerSettings = connectionSettings;

            // Initialize AuthAPIManager
            AuthAPIManager.Instance.Initialize(ServerSettings);
            try
            {
                // Authenticate Agent
                AuthenticateAgent();

                // API Call to Connect
                var connectAPIResponse = AgentsAPIManager.ConnectAgent(AuthAPIManager.Instance, ServerSettings);

                // Update Server Settings
                ServerSettings.ServerConnectionEnabled = true;
                ServerSettings.AgentId = connectAPIResponse.Data.AgentId.ToString();
                ServerSettings.AgentName = connectAPIResponse.Data.AgentName.ToString();

                // On Successful Connection with Server
                StartHeartBeatTimer();
                StartJobPolling();

                // Send Response to Agent
                return new ServerResponse(ServerSettings, connectAPIResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                // Update Server Settings
                ServerSettings.ServerConnectionEnabled = false;
                ServerSettings.AgentId = string.Empty;
                ServerSettings.AgentName = string.Empty;

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
                var apiResponse = AgentsAPIManager.DisconnectAgent(AuthAPIManager.Instance, ServerSettings);

                // Update settings
                //ServerSettings = connectionSettings;
                ServerSettings.ServerConnectionEnabled = false;
                ServerSettings.AgentId = string.Empty;
                ServerSettings.AgentName = string.Empty;

                // After Disconnecting from Server
                StopHeartBeatTimer();
                StopJobPolling();

                // Form Server Response
                return new ServerResponse(ServerSettings, apiResponse.StatusCode.ToString());
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
                if (!AgentsAPIManager.FindAgent(AuthAPIManager.Instance, $"name eq '{ServerSettings.AgentUsername}'"))
                    AgentsAPIManager.CreateAgent(AuthAPIManager.Instance, ServerSettings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion ServerConnection

    }
}
