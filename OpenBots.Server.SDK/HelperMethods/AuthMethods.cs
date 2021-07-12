using OpenBots.Server.SDK.Api;
using OpenBots.Server.SDK.Model;
using System;

namespace OpenBots.Server.SDK.HelperMethods
{
    public class AuthMethods
    {
        public AuthMethods()
        { }

        public void Initialize(string serverType, string organizationName, string serverUrl, string username, string password, string agentId = "",
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
            AgentId = agentId;
        }

        public string ServerType { get; set; }
        public string OrganizationName { get; set; }
        public string ServerUrl { get; set; }
        public string ApiVersion { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MachineUser { get; set; }
        public string UserDomainName { get; set; }
        public string AgentId { get; set; }

        public UserInfo GetUserInfo()
        {
            var apiInstance = new AuthApi(ServerUrl);

            try
            {
                var serverEnvironment = GetServerEnvironment();
                var userInfo = apiInstance.GetUserInfo(ApiVersion, ServerType, OrganizationName, serverEnvironment, ServerUrl, Username, Password, AgentId);

                return userInfo;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AuthApi.RefreshToken: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
        }

        public UserInfo GetDocumentsAuthToken(string username, string password)
        {
            var apiInstance = new AuthApi(ServerUrl);

            try
            {
                string serverType = "Documents";
                var serverEnvironment = GetServerEnvironment();
                var userInfo = apiInstance.GetUserInfo(ApiVersion, serverType, OrganizationName, serverEnvironment, ServerUrl, username, password, AgentId);

                return userInfo;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AuthApi.RefreshToken: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
            }
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

        public void RefreshToken(UserInfo userInfo)
        {
            AuthApi apiInstance = new AuthApi(userInfo.ServerUrl);

            try
            {
                var refreshModel = apiInstance.RefreshToken(userInfo);
                userInfo.Token = refreshModel.Token;
                userInfo.RefreshToken = refreshModel.RefreshToken;
            }
            catch (Exception ex)
            {
                if (ex.Message != "One or more errors occurred.")
                    throw new InvalidOperationException("Exception when calling AuthApi.RefreshToken: " + ex.Message);
                else
                    throw new InvalidOperationException(ex.InnerException.Message);
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
