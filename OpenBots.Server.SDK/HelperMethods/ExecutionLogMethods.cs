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
    public class ExecutionLogMethods
    {
        public static AutomationExecutionLog CreateExecutionLog(UserInfo userInfo, AutomationExecutionLog body)
        {
            var executionLogsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return new AutomationExecutionLog();
                //return executionLogsApi.ApiVapiVersionAutomationExecutionLogsStartAutomationPostWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, body).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AutomationExecutionLogsApi.CreateExecutionLog: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static int UpdateExecutionLog(UserInfo userInfo, AutomationExecutionLog body)
        {
            var executionLogsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return 0;
                //return executionLogsApi.ApiVapiVersionAutomationExecutionLogsIdEndAutomationPutWithHttpInfo(body.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, body).StatusCode;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
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
