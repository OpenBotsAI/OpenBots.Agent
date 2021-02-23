using OpenBots.Service.API.Api;
using OpenBots.Service.API.Model;
using System;

namespace OpenBots.Service.Client.Manager.API
{
    public static class CredentialsAPIManager
    {
        public static Credential GetCredentials(AuthAPIManager apiManager, string credentialId)
        {
            CredentialsApi credentialsApi = new CredentialsApi(apiManager.Configuration);

            try
            {
                return credentialsApi.GetCredential(credentialId);
            }
            catch (Exception ex)
            {
                // In case of Unauthorized request
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401")
                {
                    // Refresh Token and Call API
                    credentialsApi.Configuration.AccessToken = apiManager.GetToken();
                    return credentialsApi.GetCredential(credentialId);
                }
                throw ex;
            }
        }
    }
}
