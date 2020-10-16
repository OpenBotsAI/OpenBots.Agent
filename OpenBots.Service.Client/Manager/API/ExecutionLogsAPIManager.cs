using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;

namespace OpenBots.Service.Client.Manager.API
{
    public static class ExecutionLogsAPIManager
    {
        public static ProcessExecutionLog CreateExecutionLog(AuthAPIManager apiManager, ProcessExecutionLog body)
        {
            ProcessExecutionLogApi executionLogsApi = new ProcessExecutionLogApi(apiManager.Configuration);

            try
            {
                return executionLogsApi.ApiV1ProcessExecutionLogStartprocessPost(body);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    executionLogsApi.Configuration.AccessToken = apiManager.GetToken();
                    return executionLogsApi.ApiV1ProcessExecutionLogStartprocessPost(body);
                }
                throw ex;
            }
        }

        public static int UpdateExecutionLog(AuthAPIManager apiManager, ProcessExecutionLog body)
        {
            ProcessExecutionLogsApi executionLogsApi = new ProcessExecutionLogsApi(apiManager.Configuration);

            try
            {
                return executionLogsApi.ApiV1ProcessExecutionLogsIdEndProcessPutWithHttpInfo(body.Id.ToString(), body).StatusCode;
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    executionLogsApi.Configuration.AccessToken = apiManager.GetToken();
                    return executionLogsApi.ApiV1ProcessExecutionLogsIdEndProcessPutWithHttpInfo(body.Id.ToString(), body).StatusCode;
                }
                throw ex;
            }
        }
    }
}
