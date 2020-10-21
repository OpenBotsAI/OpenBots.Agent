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
        }

        public void UnInitialize()
        {
            Configuration.ApiClient = null;
        }

        public string GetToken()
        {
            AuthApi authAPI = new AuthApi(ServerSettings.ServerURL);
            var apiResponse = authAPI.ApiV1AuthTokenPostWithHttpInfo(new LoginModel(ServerSettings.AgentUsername, ServerSettings.AgentPassword));
            
            return (Configuration.AccessToken = apiResponse.Data.Token.ToString());
        }

        public void RegisterAgentUser()
        {
            AuthApi authAPI = new AuthApi(ServerSettings.ServerURL);
            var signupModel = new SignUpViewModel(ServerSettings.AgentUsername,null, null, null,  
                ServerSettings.AgentPassword, false, null, null, null, null, null, null, null);
            
            var apiResponse = authAPI.ApiV1AuthRegisterPostWithHttpInfo(signupModel);
        }
    }
}
