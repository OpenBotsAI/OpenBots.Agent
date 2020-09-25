using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;

namespace OpenBots.Service.Client.Manager
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
                // Refresh Token and Call API
                jobsApi.Configuration.AccessToken = apiManager.GetNewToken();
                return jobsApi.ApiV1JobsGetWithHttpInfo(filter);
            }
        }
    }
}
