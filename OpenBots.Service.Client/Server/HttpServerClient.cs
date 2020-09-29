using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;
using System.Timers;
using OpenBots.Service.Client.Executor;

namespace OpenBots.Service.Client.Server
{
    public class HttpServerClient
    {
        private Timer _heartbeatTimer, _jobsFetchTimer;
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
                    ServerSettings.ServerURL,
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

        private void StartJobsFetchTimer()
        {
            if (ServerSettings.ServerConnectionEnabled)
            {
                //handle for reinitialization
                if (_jobsFetchTimer != null)
                {
                    _jobsFetchTimer.Elapsed -= JobsFetchTimer_Elapsed;
                }

                //setup heartbeat to the server
                _jobsFetchTimer = new Timer();
                _jobsFetchTimer.Interval = 30000;
                _jobsFetchTimer.Elapsed += JobsFetchTimer_Elapsed;
                _jobsFetchTimer.Enabled = true;

                // Start Execution Manager to Run Jobs
                ExecutionManager.Instance.StartNewJobsCheckTimer();
            }
        }

        private void StopJobsFetchTimer()
        {
            if (_jobsFetchTimer != null)
            {
                _jobsFetchTimer.Enabled = false;
                _jobsFetchTimer.Elapsed -= JobsFetchTimer_Elapsed;

                // Stop Execution Manager
                ExecutionManager.Instance.StopNewJobsCheckTimer();
            }
        }

        private void JobsFetchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if(ServerSettings.ServerConnectionEnabled)
                {
                    //Retrieve New Jobs for this Agent
                    var apiResponse = JobsAPIManager.GetJobs(
                        AuthAPIManager.Instance,
                        $"agentId eq guid'{ServerSettings.AgentId}' and jobStatus eq 'New'");

                    if (apiResponse.Data.Items.Count != 0)
                        foreach (var job in apiResponse.Data.Items)
                            JobsQueueManager.Instance.EnqueueJob(job);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void UnInitialize()
        {
            StopHeartBeatTimer();
            StopJobsFetchTimer();
        }

        public Boolean IsConnected()
        {
            return ServerSettings?.ServerConnectionEnabled ?? false;
        }

        public ServerResponse Connect(ServerConnectionSettings connectionSettings)
        {
            ServerSettings = connectionSettings;
            try
            {
                // API Call to Connect
                var apiResponse = AgentsAPIManager.ConnectAgent(
                    ServerSettings.ServerURL,
                    ServerSettings.DNSHost,
                    ServerSettings.MACAddress
                    );

                // Update Server Settings
                ServerSettings.ServerConnectionEnabled = true;
                ServerSettings.AgentId = apiResponse.Data.AgentId.ToString();
                ServerSettings.AgentName = apiResponse.Data.AgentName.ToString();

                // Initialize AuthAPIManager
                AuthAPIManager.Instance.Initialize(ServerSettings);

                // On Successful Connection with Server
                StartHeartBeatTimer();
                StartJobsFetchTimer();

                // Send Response to Agent
                return new ServerResponse(ServerSettings, apiResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                // Update Server Settings
                ServerSettings.ServerConnectionEnabled = false;
                ServerSettings.AgentId = string.Empty;
                ServerSettings.AgentName = string.Empty;

                // Send Response to Agent
                return new ServerResponse(null,
                    ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString(),
                    ex.GetType().GetProperty("ErrorContent").GetValue(ex, null).ToString());
            }
        }

        public ServerResponse Disconnect(ServerConnectionSettings connectionSettings)
        {
            try
            {
                // API Call to Disconnect
                var apiResponse = AgentsAPIManager.DisconnectAgent(
                        ServerSettings.ServerURL,
                        ServerSettings.DNSHost,
                        ServerSettings.MACAddress,
                        ServerSettings.AgentId
                        );

                // Update settings
                //ServerSettings = connectionSettings;
                ServerSettings.ServerConnectionEnabled = false;
                ServerSettings.AgentId = string.Empty;
                ServerSettings.AgentName = string.Empty;

                // After Disconnecting from Server
                StopHeartBeatTimer();
                StopJobsFetchTimer();

                // Form Server Response
                return new ServerResponse(ServerSettings, apiResponse.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                // Form Server Response
                return new ServerResponse(null,
                    ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString(),
                    ex.GetType().GetProperty("ErrorContent").GetValue(ex, null).ToString());
            }
        }
    }
}
