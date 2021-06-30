using Newtonsoft.Json;
using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Client;
using OpenBots.Server.SDK.Model;
using System;
using SDKCredential = OpenBots.Server.SDK.Model.Credential;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class CredentialMethods
    {
        public static Credential GetCredential(string token, string serverUrl, string organizationId, string name, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                var result = apiInstance.ApiVapiVersionCredentialsGetCredentialByNameCredentialNameGetAsyncWithHttpInfo(name, apiVersion, organizationId).Result.Data;

                return result;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling CredentialsApi.GetCredentialByName: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static void PutCredential(string token, string serverUrl, string organizationId, Credential credential, string apiVersion)
        {
            var apiInstance = GetApiInstance(token, serverUrl);

            try
            {
                apiInstance.ApiVapiVersionCredentialsIdPutAsyncWithHttpInfo(credential.Id.ToString(), apiVersion, organizationId, credential).Wait();
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling CredentialsApi.UpdateCredential: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public static Credential GetCredentials(UserInfo userInfo, string credentialId)
        {
            var credentialsApi = GetApiInstance(userInfo.Token, userInfo.ServerUrl);

            try
            {
                return credentialsApi.GetCredentialWithHttpInfo(credentialId, userInfo.ApiVersion, userInfo.OrganizationId).Data;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
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
