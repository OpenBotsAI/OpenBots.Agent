using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Client;
using OpenBots.Server.SDK.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class AgentMethods
    {
        public static NextJobViewModel SendAgentHeartBeat(UserInfo userInfo, string agentId, HeartbeatViewModel body)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.ApiVapiVersionAgentsAgentIdAddHeartbeatPostWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId, body).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.SendAgentHeartBeat: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static ResolvedAgentResponseViewModel ResolveAgent(UserInfo userInfo, ResolveAgentViewModel body)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.ApiVapiVersionAgentsResolvePostWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, body).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.ResolveAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static ConnectedViewModel ConnectAgent(UserInfo userInfo, string agentId, ConnectAgentViewModel connectAgentViewModel)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.ApiVapiVersionAgentsAgentIDConnectPatchWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId, connectAgentViewModel).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.ConnectAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DisconnectAgent(UserInfo userInfo, string agentId, ConnectAgentViewModel connectAgentViewModel)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);
            try
            {
                agentsApi.ApiVapiVersionAgentsAgentIDDisconnectPatchWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId, connectAgentViewModel);
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.DisconnectAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static AgentViewModel GetAgent(UserInfo userInfo, string agentId)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.GetAgentWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.GetAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        private static AgentsApi GetApiInstance(string token, string serverUrl)
        {
            var apiInstance = new AgentsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            return apiInstance;
        }
    }
}
