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
    public class JobMethods
    {
        public static Job UpdateJobStatus(UserInfo userInfo, string agentId, string jobId, JobStatusType status, JobErrorViewModel errorModel = null)
        {
            var jobsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return jobsApi.ApiVapiVersionJobsIdStatusStatusPutWithHttpInfo(agentId, jobId, status, userInfo.ApiVersion, userInfo.OrganizationId, errorModel).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling JobsApi.UpdateJobStatus: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static int UpdateJobPatch(UserInfo userInfo, string id, List<Operation> body)
        {
            var jobsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return jobsApi.ApiVapiVersionJobsIdPatchWithHttpInfo(id, userInfo.ApiVersion, userInfo.OrganizationId, body).StatusCode;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling JobsApi.UpdateJobPatch: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static JobViewModel GetJobViewModel(UserInfo userInfo, string jobId)
        {
            var jobsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return jobsApi.ApiVapiVersionJobsViewIdGetWithHttpInfo(jobId, userInfo.ApiVersion, userInfo.OrganizationId).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling JobsApi.GetJobViewModel: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        private static JobsApi GetApiInstance(string token, string serverUrl)
        {
            var apiInstance = new JobsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            return apiInstance;
        }

    }
}
