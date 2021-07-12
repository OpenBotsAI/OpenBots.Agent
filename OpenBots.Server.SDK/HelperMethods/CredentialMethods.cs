using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class CredentialMethods
    {
        public static Credential GetCredential(UserInfo userInfo, string name, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                var result = apiInstance.ApiVapiVersionCredentialsGetCredentialByNameCredentialNameGetAsyncWithHttpInfo(name, userInfo.ApiVersion, userInfo.OrganizationId).Result.Data;

                return result;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetCredential(userInfo, name, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling CredentialsApi.GetCredentialByName: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void PutCredential(UserInfo userInfo, Credential credential, int count = 0)
        {
            var apiInstance = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                apiInstance.ApiVapiVersionCredentialsIdPutAsyncWithHttpInfo(credential.Id.ToString(), userInfo.ApiVersion, userInfo.OrganizationId, credential).Wait();
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    PutCredential(userInfo, credential, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling CredentialsApi.UpdateCredential: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static Credential GetCredentials(UserInfo userInfo, string credentialId, int count = 0)
        {
            var credentialsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return credentialsApi.GetCredentialWithHttpInfo(credentialId, userInfo.ApiVersion, userInfo.OrganizationId).Data;
            }
            catch (Exception ex)
            {
                if (ex.GetType().GetProperty("ErrorCode").GetValue(ex, null).ToString() == "401" && count < 2)
                {
                    UtilityMethods.RefreshToken(userInfo);
                    count++;
                    return GetCredentials(userInfo, credentialId, count);
                }
                else if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling CredentialsApi.GetCredentials: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        private static CredentialsApi GetApiInstance(string token, string serverUrl)
        {
            var apiInstance = new CredentialsApi(serverUrl);
            apiInstance.Configuration.AccessToken = token;

            return apiInstance;
        }
    }
}