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

        public static ApiResponse<IActionResult> UpdateJob(AuthAPIManager apiManager, string jobId, List<Operation> body)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsIdPatchWithHttpInfo(jobId, body);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsIdPatchWithHttpInfo(jobId, body);
                }
                throw ex;
            }
        }
    }
}
