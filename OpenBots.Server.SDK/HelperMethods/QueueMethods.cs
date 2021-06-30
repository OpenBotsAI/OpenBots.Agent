using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;
using System.Linq;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class QueueMethods
    {
        public static Queue GetQueue(string token, string serverUrl, string organizationId, string filter, string apiVersion)
        {
            var apiInstance = new QueuesApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            try
            {
                var queue = apiInstance.ApiVapiVersionQueuesGetAsyncWithHttpInfo(apiVersion, organizationId, filter).Result.Data.Items.FirstOrDefault();
                return queue;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueuesApi.GetQueues: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }
    }
}
