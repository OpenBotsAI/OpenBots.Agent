using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;
using IO = System.IO;

namespace OpenBots.Service.Client.Manager.API
{
    public static class AutomationsAPIManager
    {
        public static ApiResponse<IO.MemoryStream> ExportAutomation(AuthAPIManager apiManager, string automationID)
        {
            AutomationsApi automationsApi = new AutomationsApi(apiManager.Configuration);

            try
            {
                return automationsApi.ExportAutomationWithHttpInfo(automationID);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    automationsApi.Configuration.AccessToken = apiManager.GetToken();
                    return automationsApi.ExportAutomationWithHttpInfo(automationID);
                }
                throw ex;
            }
        }

        public static Automation GetAutomation(AuthAPIManager apiManager, string automationID)
        {
            AutomationsApi automationsApi = new AutomationsApi(apiManager.Configuration);

            try
            {
                return automationsApi.GetAutomationWithHttpInfo(automationID).Data;
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    automationsApi.Configuration.AccessToken = apiManager.GetToken();
                    return automationsApi.GetAutomationWithHttpInfo(automationID).Data;
                }
                throw ex;
            }
        }
    }
}
