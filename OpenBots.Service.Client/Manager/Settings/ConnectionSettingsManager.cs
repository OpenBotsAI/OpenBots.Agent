using OpenBots.Agent.Core.Model;
using System;

namespace OpenBots.Service.Client.Manager.Settings
{
    public class ConnectionSettingsManager
    {
        public ServerConnectionSettings ConnectionSettings { get; set; }
        public event EventHandler<ServerConnectionSettings> ConnectionSettingsUpdatedEvent;
        public ConnectionSettingsManager()
        {
        }

        public void UpdateConnectionSettings(ServerConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;
            ConnectionSettingsUpdatedEvent?.Invoke(this, ConnectionSettings);
        }
    }
}
