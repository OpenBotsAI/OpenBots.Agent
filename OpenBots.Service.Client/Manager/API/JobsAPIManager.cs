using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using System.Collections.Generic;

namespace OpenBots.Service.Client.Manager.API
{
    public static class JobsAPIManager
    {
        public static ApiResponse<NextJobViewModel> GetJob(AuthAPIManager apiManager, string agentId)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsNextGetWithHttpInfo(agentId);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsNextGetWithHttpInfo(agentId);
                }
                throw ex;
            }
        }

        public static ApiResponse<Job> UpdateJobStatus(AuthAPIManager apiManager, string agentId, string jobId, JobStatusType status, JobErrorViewModel errorModel = null)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsIdStatusStatusPutWithHttpInfo(agentId, jobId, status, errorModel);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsIdStatusStatusPutWithHttpInfo(agentId, jobId, status, errorModel);
                }
                throw ex;
            }
        }

        public static int UpdateJobPatch(AuthAPIManager apiManager, string id, List<Operation> body)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsIdPatchWithHttpInfo(id, body).StatusCode;
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsIdPatchWithHttpInfo(id, body).StatusCode;
                }
                throw ex;
            }
        }

        public static JobViewModel GetJobViewModel(AuthAPIManager apiManager, string jobId)
        {
            JobsApi jobsApi = new JobsApi(apiManager.Configuration);

            try
            {
                return jobsApi.ApiV1JobsViewIdGet(jobId);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    jobsApi.Configuration.AccessToken = apiManager.GetToken();
                    return jobsApi.ApiV1JobsViewIdGet(jobId);
                }
                throw ex;
            }
        }
    }
}
