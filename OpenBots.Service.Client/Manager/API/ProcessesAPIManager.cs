using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using IO = System.IO;

namespace OpenBots.Service.Client.Manager.API
{
    public static class ProcessesAPIManager
    {
        public static ApiResponse<IO.MemoryStream> ExportProcess(AuthAPIManager apiManager, string processID)
        {
            ProcessesApi processesApi = new ProcessesApi(apiManager.Configuration);

            try
            {
                return processesApi.ExportProcessWithHttpInfo(processID);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    processesApi.Configuration.AccessToken = apiManager.GetToken();
                    return processesApi.ExportProcessWithHttpInfo(processID);
                }
                throw ex;
            }
        }

        public static Process GetProcess(AuthAPIManager apiManager, string processID)
        {
            ProcessesApi processesApi = new ProcessesApi(apiManager.Configuration);

            try
            {
                return processesApi.GetProcessWithHttpInfo(processID).Data;
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    processesApi.Configuration.AccessToken = apiManager.GetToken();
                    return processesApi.GetProcessWithHttpInfo(processID).Data;
                }
                throw ex;
            }
        }
    }
}
