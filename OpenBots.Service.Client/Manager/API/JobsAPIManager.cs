using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;

namespace OpenBots.Service.Client.Manager.API
{
    public static class JobsAPIManager
    {
        public static ApiResponse<JobPaginatedList> GetJobs(AuthAPIManager apiManager, string filter)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsGetWithHttpInfo(filter);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsGetWithHttpInfo(filter);
                }
                throw ex;
            }
        }

        public static ApiResponse<Job> UpdateJobStatus(AuthAPIManager apiManager, string jobId, JobStatusType status, string agentId, JobErrorViewModel errorModel = null)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsIdStatusStatusPutWithHttpInfo(jobId, status, agentId);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsIdStatusStatusPutWithHttpInfo(jobId, status, agentId);
                }
                throw ex;
            }
        }
    }
}
