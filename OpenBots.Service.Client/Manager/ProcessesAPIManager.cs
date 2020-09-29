using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using System;
using System.IO;

namespace OpenBots.Service.Client.Manager
{
    public static class ProcessesAPIManager
    {
        public static ApiResponse<MemoryStream> ExportProcess(AuthAPIManager apiManager, string processID)
        {
            ProcessesApi processesApi = new ProcessesApi(apiManager.Configuration);

            try
            {
                return processesApi.ExportProcessWithHttpInfo(processID);
            }
            catch (Exception ex)
            {
                // Refresh Token and Call API
                processesApi.Configuration.AccessToken = apiManager.GetNewToken();
                return processesApi.ExportProcessWithHttpInfo(processID);
            }
        }
    }
}
