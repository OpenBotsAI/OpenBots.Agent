using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class AuthMethods
    {
        public AuthMethods(string serverType, string organizationName, string serverUrl, string username, string password,
             string machineUserName = "", string userDomainName = "", string apiVersion = "1")
        {
            ServerType = serverType;
            OrganizationName = organizationName;
            ServerUrl = serverUrl;
            ApiVersion = apiVersion;
            Username = username;
            Password = password;
            MachineUser = machineUserName;
            UserDomainName = userDomainName;
        }

        public string ServerType { get; set; }
        public string OrganizationName { get; set; }
        public string ServerUrl { get; set; }
        public string ApiVersion { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MachineUser { get; set; }
        public string UserDomainName { get; set; }

        public UserInfo GetUserInfo()
        {
            var apiInstance = new AuthApi(ServerUrl);

            var serverEnvironment = GetServerEnvironment();
            var userInfo = apiInstance.GetUserInfo(ApiVersion, ServerType, OrganizationName, serverEnvironment, ServerUrl, Username, Password);

            return userInfo;
        }

        public UserInfo GetDocumentsAuthToken(string username, string password)
        {
            var apiInstance = new AuthApi(ServerUrl);

            string serverType = "Documents";
            var serverEnvironment = GetServerEnvironment();
            var userInfo = apiInstance.GetUserInfo(ApiVersion, serverType, OrganizationName, serverEnvironment, ServerUrl, username, password);

            return userInfo;
        }

        public string Ping()
        {
            try
            {
                AuthApi authAPI = new AuthApi(ServerUrl);
                var serverIP = authAPI.ApiVapiVersionAuthPingGetWithHttpInfo(ApiVersion).Data;

                if (!string.IsNullOrEmpty(serverIP))
                    serverIP = serverIP.Replace("\"", "");
                return serverIP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetServerEnvironment()
        {
            try
            {
                string environment = Environment.GetEnvironmentVariable("OpenBots_Environment", EnvironmentVariableTarget.Machine);
                if (string.IsNullOrEmpty(environment))
                    environment = Environment.GetEnvironmentVariable("OpenBots_Environment", EnvironmentVariableTarget.User);
                if (string.IsNullOrEmpty(environment))
                    environment = UtilityMethods.GetUserEnvironmentVariable(UserDomainName, MachineUser, "OpenBots_Environment");
                if (string.IsNullOrEmpty(environment))
                    environment = "LIVE";

                return environment;
            }
            catch (Exception)
            {
                return "LIVE";
            }
        }
    }
}
