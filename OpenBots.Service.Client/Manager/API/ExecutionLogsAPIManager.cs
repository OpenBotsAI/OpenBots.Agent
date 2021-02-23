using OpenBots.Service.API.Api;
using OpenBots.Service.API.Model;
using System;

namespace OpenBots.Service.Client.Manager.API
{
    public static class ExecutionLogsAPIManager
    {
        public static AutomationExecutionLog CreateExecutionLog(AuthAPIManager apiManager, AutomationExecutionLog body)
        {
            AutomationExecutionLogsApi executionLogsApi = new AutomationExecutionLogsApi(apiManager.Configuration);

            try
            {
                return executionLogsApi.ApiV1AutomationExecutionLogsStartAutomationPost(body);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    executionLogsApi.Configuration.AccessToken = apiManager.GetToken();
                    return executionLogsApi.ApiV1AutomationExecutionLogsStartAutomationPost(body);
                }
                throw ex;
            }
        }

        public static int UpdateExecutionLog(AuthAPIManager apiManager, AutomationExecutionLog body)
        {
            AutomationExecutionLogsApi executionLogsApi = new AutomationExecutionLogsApi(apiManager.Configuration);

            try
            {
                return executionLogsApi.ApiV1AutomationExecutionLogsIdEndAutomationPutWithHttpInfo(body.Id.ToString(), body).StatusCode;
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    executionLogsApi.Configuration.AccessToken = apiManager.GetToken();
                    return executionLogsApi.ApiV1AutomationExecutionLogsIdEndAutomationPutWithHttpInfo(body.Id.ToString(), body).StatusCode;
                }
                throw ex;
            }
        }
    }
}
