using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using System.IO;
using System.Linq;

namespace OpenBots.Service.Client.Manager.API
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
            catch (Exception)
            {
                // Refresh Token and Call API
                processesApi.Configuration.AccessToken = apiManager.GetToken();
                return processesApi.ExportProcessWithHttpInfo(processID);
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
                // Refresh Token and Call API
                processesApi.Configuration.AccessToken = apiManager.GetToken();
                return processesApi.GetProcessWithHttpInfo(processID).Data;
            }
        }
    }
}
