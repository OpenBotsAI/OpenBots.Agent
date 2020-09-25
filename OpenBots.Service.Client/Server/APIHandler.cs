using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;

namespace OpenBots.Service.Client.Server
{
    public static class APIHandler
    {
        //public static int SendAgentHeartBeat(string baseURL, string agentId, List<Operation> body)
        //{
        //    AgentsApi agentsApi = new AgentsApi(baseURL);
        //    return agentsApi.ApiV1AgentsIdHeartbeatPatchWithHttpInfo(agentId, body).StatusCode;
        //}

        //public static ApiResponse<ConnectAgentResponseModel> ConnectAgent(string baseURL, string MachineName, string MACAddress)
        //{
        //    AgentsApi agentsApi = new AgentsApi(baseURL);
        //    return agentsApi.ApiV1AgentsConnectPatchWithHttpInfo(MachineName, MACAddress);
        //}

        //public static ApiResponse<IActionResult> DisconnectAgent(string baseURL, string MachineName, string MACAddress, string AgentId)
        //{
        //    AgentsApi agentsApi = new AgentsApi(baseURL);
        //    return agentsApi.ApiV1AgentsDisconnectPatchWithHttpInfo(MachineName, MACAddress, new Guid(AgentId));
        //}

        //public static ApiResponse<JobPaginatedList> GetJobs(string baseURL, string filter)
        //{
        //    JobsApi jobsApi = new JobsApi(baseURL);
        //    return jobsApi.ApiV1JobsGetWithHttpInfo(filter);
        //}
    }
}
