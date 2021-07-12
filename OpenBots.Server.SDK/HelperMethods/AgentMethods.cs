using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class AgentMethods
    {
        public static NextJobViewModel SendAgentHeartBeat(UserInfo userInfo, string agentId, HeartbeatViewModel body, int count = 0)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.ApiVapiVersionAgentsAgentIdAddHeartbeatPostWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId, body).Data;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return SendAgentHeartBeat(userInfo, agentId, body, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.SendAgentHeartBeat: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static ResolvedAgentResponseViewModel ResolveAgent(UserInfo userInfo, ResolveAgentViewModel body, int count = 0)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.ApiVapiVersionAgentsResolvePostWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, body).Data;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return ResolveAgent(userInfo, body, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.ResolveAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static ConnectedViewModel ConnectAgent(UserInfo userInfo, string agentId, ConnectAgentViewModel connectAgentViewModel, int count = 0)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.ApiVapiVersionAgentsAgentIDConnectPatchWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId, connectAgentViewModel).Data;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return ConnectAgent(userInfo, agentId, connectAgentViewModel, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.ConnectAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void DisconnectAgent(UserInfo userInfo, string agentId, ConnectAgentViewModel connectAgentViewModel, int count = 0)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);
            try
            {
                agentsApi.ApiVapiVersionAgentsAgentIDDisconnectPatchWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId, connectAgentViewModel);
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    DisconnectAgent(userInfo, agentId, connectAgentViewModel, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AgentsApi.DisconnectAgent: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static AgentViewModel GetAgent(UserInfo userInfo, string agentId, int count = 0)
        {
            var agentsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return agentsApi.GetAgentWithHttpInfo(agentId, userInfo.ApiVersion, userInfo.OrganizationId).Data;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetAgent(userInfo, agentId, count);
                }
                else if (ex.Message != "One or more errors occurred.")
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
