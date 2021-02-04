using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;
using System;

namespace OpenBots.Service.Client.Manager
{
    public class AuthAPIManager
    {
        public Configuration Configuration { get; set; }
        public ServerConnectionSettings ConnectionSettings { get; set; }

        public event EventHandler<Configuration> ConfigurationUpdatedEvent;
        public AuthAPIManager()
        {
            Configuration = new Configuration();
        }

        public void Initialize(ServerConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;
            Configuration.BasePath = ConnectionSettings.ServerURL;
        }

        public void UnInitialize()
        {
            Configuration.ApiClient = null;
        }

        public string GetToken()
        {
            AuthApi authAPI = new AuthApi(ConnectionSettings.ServerURL);
            var apiResponse = authAPI.ApiV1AuthTokenPostWithHttpInfo(new LoginModel(ConnectionSettings.AgentUsername, ConnectionSettings.AgentPassword));

            Configuration.AccessToken = apiResponse.Data.Token.ToString();
            ConfigurationUpdatedEvent?.Invoke(this, Configuration);

            ConnectionSettings.AgentId = apiResponse.Data.AgentId;
            return Configuration.AccessToken;
        }

        public string Ping()
        {
            try
            {
                AuthApi authAPI = new AuthApi(Configuration);
                var serverIP = authAPI.ApiV1AuthPingGet();

                if (!string.IsNullOrEmpty(serverIP))
                    serverIP = serverIP.Replace("\"", "");
                return serverIP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
