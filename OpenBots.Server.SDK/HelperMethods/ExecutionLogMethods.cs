using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class ExecutionLogMethods
    {
        public static AutomationExecutionLog CreateExecutionLog(UserInfo userInfo, AutomationExecutionLog body, int count = 0)
        {
            var executionLogsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return executionLogsApi.ApiVapiVersionAutomationExecutionLogsStartAutomationPostWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, body).Data;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return CreateExecutionLog(userInfo, body, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AutomationExecutionLogsApi.CreateExecutionLog: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static int UpdateExecutionLog(UserInfo userInfo, AutomationExecutionLog body, int count = 0)
        {
            var executionLogsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return executionLogsApi.ApiVapiVersionAutomationExecutionLogsIdEndAutomationPutWithHttpInfo(body.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, body).StatusCode;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return UpdateExecutionLog(userInfo, body, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AutomationExecutionLogsApi.UpdateExecutionLog: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        private static AutomationExecutionLogsApi GetApiInstance(string token, string serverUrl)
        {
            var apiInstance = new AutomationExecutionLogsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            return apiInstance;
        }
    }
}
