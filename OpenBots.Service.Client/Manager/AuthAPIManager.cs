using OpenBots.Agent.Core.Model;
using OpenBots.Service.API.Api;
using OpenBots.Service.API.Client;
using OpenBots.Service.API.Model;

namespace OpenBots.Service.Client.Manager
{
    public class AuthAPIManager
    {
        public Configuration Configuration { get; private set; }
        public ServerConnectionSettings ServerSettings { get; private set; }
        public static AuthAPIManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AuthAPIManager();

                return instance;
            }
        }
        private static AuthAPIManager instance;

        private AuthAPIManager()
        {
            Configuration = new Configuration();
        }

        public void Initialize(ServerConnectionSettings serverSettings)
        {
            ServerSettings = serverSettings;
            Configuration.BasePath = ServerSettings.ServerURL;
            GetNewToken();
        }

        public string GetNewToken()
        {
            AuthApi authAPI = new AuthApi(ServerSettings.ServerURL);
            //var token = authAPI.ApiV1AuthTokenPostWithHttpInfo(new LoginModel() 
            //{ 
            //    Email = ServerSettings.AgentName, 
            //    Password = ServerSettings.AgentId 
            //});
            var apiResponse = authAPI.ApiV1AuthAgentTokenPostWithHttpInfo(new VerifyAgentModel(ServerSettings.AgentId, ServerSettings.DNSHost, ServerSettings.MACAddress));
            return (Configuration.AccessToken = apiResponse.Data.Token.ToString());
        }
    }
}
