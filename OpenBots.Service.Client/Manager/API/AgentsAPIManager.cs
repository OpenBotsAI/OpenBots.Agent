using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;

namespace OpenBots.Service.Client.Manager.API
{
    public static class AgentsAPIManager
    {
        public static ApiResponse<NextJobViewModel> SendAgentHeartBeat(AuthAPIManager apiManager, string agentId, HeartbeatViewModel body)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);

            try
            {
                return agentsApi.ApiV1AgentsAgentIdAddHeartbeatPostWithHttpInfo(agentId, body);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    agentsApi.Configuration.AccessToken = apiManager.GetToken();
                    return agentsApi.ApiV1AgentsAgentIdAddHeartbeatPostWithHttpInfo(agentId, body);
                }
                throw ex;
            }
        }

        public static ApiResponse<ConnectedViewModel> ConnectAgent(AuthAPIManager apiManager, ServerConnectionSettings serverSettings)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);

            try
            {
                return agentsApi.ApiV1AgentsAgentIDConnectPatchWithHttpInfo(serverSettings.AgentId, serverSettings.MachineName, serverSettings.MACAddress);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    agentsApi.Configuration.AccessToken = apiManager.GetToken();
                    return agentsApi.ApiV1AgentsAgentIDConnectPatchWithHttpInfo(serverSettings.AgentId, serverSettings.MachineName, serverSettings.MACAddress);
                }
                throw ex;
            }
        }

        public static ApiResponse<IActionResult> DisconnectAgent(AuthAPIManager apiManager, ServerConnectionSettings serverSettings)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);
            try
            {
                return agentsApi.ApiV1AgentsAgentIDDisconnectPatchWithHttpInfo(serverSettings.AgentId, serverSettings.MachineName, serverSettings.MACAddress);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    agentsApi.Configuration.AccessToken = apiManager.GetToken();
                    return agentsApi.ApiV1AgentsAgentIDDisconnectPatchWithHttpInfo(serverSettings.AgentId, serverSettings.MachineName, serverSettings.MACAddress);
                }
                throw ex;
            }
        }

        public static AgentViewModel GetAgent(AuthAPIManager apiManager, string agentId)
        {
            AgentsApi agentsApi = new AgentsApi(apiManager.Configuration);

            try
            {
                return agentsApi.GetAgent(agentId);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    agentsApi.Configuration.AccessToken = apiManager.GetToken();
                    return agentsApi.GetAgent(agentId);
                }
                throw ex;
            }
        }
    }
}
