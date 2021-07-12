using OpenBots.Server.SDK.HelperMethods;
using OpenBots.Server.SDK.Model;

namespace OpenBots.Service.Client.Manager.Common
{
    public class AuthAPIManager
    {
        public AuthMethods AuthMethods { get; set; }
        public UserInfo UserInfo { get; set; }

        public AuthAPIManager()
        {
            AuthMethods = new AuthMethods();
        }
    }
}
