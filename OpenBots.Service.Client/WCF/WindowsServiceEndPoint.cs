using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Server;
using System;

namespace OpenBots.Service.Client
{
    public class WindowsServiceEndPoint : IWindowsServiceEndPoint
    {
        public ServerResponse ConnectToServer(ServerConnectionSettings settings)
        {
            return HttpServerClient.Instance.Connect(settings);
        }

        public ServerResponse DisconnectFromServer(ServerConnectionSettings settings)
        {
            return HttpServerClient.Instance.Disconnect(settings);
        }

        public ServerConnectionSettings GetConnectionSettings()
        {
            return ConnectionSettingsManager.Instance?.ConnectionSettings ?? null;
        }

        public bool IsAlive()
        {
            return ServiceController.Instance.IsServiceAlive;
        }

        public bool IsConnected()
        {
            return ConnectionSettingsManager.Instance?.ConnectionSettings?.ServerConnectionEnabled ?? false;
        }

        public void SetEnvironmentVariable(string environmentVariable, string settingsFilePath)
        {
            try
            {
                Environment.SetEnvironmentVariable(environmentVariable, settingsFilePath, EnvironmentVariableTarget.Machine);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
