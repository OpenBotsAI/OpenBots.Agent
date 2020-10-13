using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;

namespace OpenBots.Service.Client.Manager.API
{
    public static class AgentsAPIManager
    {
        public static int SendAgentHeartBeat(AuthAPIManager apiManager, string agentId, List<Operation> body)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);

            try
            {
                return agentsApi.ApiV1AgentsIdHeartbeatPatchWithHttpInfo(agentId, body).StatusCode;
            }
            catch (Exception)
            {
                // Refresh Token and Call API
                agentsApi.Configuration.AccessToken = apiManager.GetToken();
                return agentsApi.ApiV1AgentsIdHeartbeatPatchWithHttpInfo(agentId, body).StatusCode;
            }
        }

        public static ApiResponse<ConnectAgentResponseModel> ConnectAgent(AuthAPIManager apiManager, ServerConnectionSettings serverSettings)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);

            try
            {
                return agentsApi.ApiV1AgentsConnectPatchWithHttpInfo(serverSettings.MachineName, serverSettings.MACAddress);
            }
            catch (Exception)
            {
                // Refresh Token and Call API
                agentsApi.Configuration.AccessToken = apiManager.GetToken();
                return agentsApi.ApiV1AgentsConnectPatchWithHttpInfo(serverSettings.MachineName, serverSettings.MACAddress);
            }
        }

        public static ApiResponse<IActionResult> DisconnectAgent(AuthAPIManager apiManager, ServerConnectionSettings serverSettings)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);
            try
            {
                return agentsApi.ApiV1AgentsDisconnectPatchWithHttpInfo(serverSettings.MachineName, serverSettings.MACAddress, new Guid(serverSettings.AgentId));
            }
            catch (Exception)
            {
                // Refresh Token and Call API
                agentsApi.Configuration.AccessToken = apiManager.GetToken();
                return agentsApi.ApiV1AgentsDisconnectPatchWithHttpInfo(serverSettings.MachineName, serverSettings.MACAddress, new Guid(serverSettings.AgentId));
            }
        }

        public static string CreateAgent(AuthAPIManager apiManager, ServerConnectionSettings serverSettings)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);
            var agentModel = new AgentModel(serverSettings.AgentUsername, null, serverSettings.MachineName, serverSettings.MACAddress,
                serverSettings.IPAddress, true, null, null, null, null, null, false);
            
            try
            {
                return agentsApi.ApiV1AgentsPostWithHttpInfo(agentModel).Data.Id.ToString();
            }
            catch (Exception)
            {
                // Refresh Token and Call API
                agentsApi.Configuration.AccessToken = apiManager.GetToken();
                return agentsApi.ApiV1AgentsPostWithHttpInfo(agentModel).Data.Id.ToString();
            }
        }

        public static bool FindAgent(AuthAPIManager apiManager, string filter)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);

            try
            {
                return (agentsApi.ApiV1AgentsGetWithHttpInfo(filter).Data.Items.Count == 0) ? false : true;
            }
            catch (Exception)
            {
                // Refresh Token and Call API
                agentsApi.Configuration.AccessToken = apiManager.GetToken();
                return (agentsApi.ApiV1AgentsGetWithHttpInfo(filter).Data.Items.Count == 0) ? false : true;
            }
        }
    }
}
