using OpenBots.Agent.Core.Model;

namespace OpenBots.Service.Client.Server
{
    public class ConnectionSettingsManager
    {
        public ServerConnectionSettings ConnectionSettings { get; set; }
        public static ConnectionSettingsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConnectionSettingsManager();

                return instance;
            }
        }
        private static ConnectionSettingsManager instance;

        private ConnectionSettingsManager()
        {
        }

        public void Initialize()
        {
            ConnectionSettings = new ServerConnectionSettings();
        }
    }
}
