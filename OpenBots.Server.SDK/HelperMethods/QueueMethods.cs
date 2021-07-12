using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;
using System.Linq;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class QueueMethods
    {
        public static Queue GetQueue(UserInfo userInfo, string filter, int count = 0)
        {
            var apiInstance = new QueuesApi(userInfo.ServerUrl);
            apiInstance.Configuration.AccessToken = userInfo.Token;

            try
            {
                var queue = apiInstance.ApiVapiVersionQueuesGetAsyncWithHttpInfo(userInfo.ApiVersion, userInfo.OrganizationId, filter).Result.Data.Items.FirstOrDefault();
                return queue;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetQueue(userInfo, filter, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling QueuesApi.GetQueues: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }
    }
}
