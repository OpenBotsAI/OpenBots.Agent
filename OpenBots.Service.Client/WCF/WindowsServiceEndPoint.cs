using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager;
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

        public ServerResponse PingServer(ServerConnectionSettings serverSettings)
        {
            try
            {
                AuthAPIManager.Instance.Initialize(serverSettings);
                var serverIP = AuthAPIManager.Instance.Ping();
                AuthAPIManager.Instance.UnInitialize();

                return new ServerResponse(serverIP);
            }
            catch (Exception ex)
            {
                var errorCode = ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty;
                var errorMessage = ex.GetType().GetProperty("ErrorContent")?.GetValue(ex, null)?.ToString() ?? ex.Message;

                // Send Response to Agent
                return new ServerResponse(null, errorCode, errorMessage);
            }
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
