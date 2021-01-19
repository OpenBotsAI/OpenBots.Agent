using OpenBots.Agent.Core.Model;

namespace OpenBots.Agent.Client.Settings
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
    }
}
