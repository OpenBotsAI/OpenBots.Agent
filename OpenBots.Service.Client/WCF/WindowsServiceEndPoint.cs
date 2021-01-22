using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager;
using OpenBots.Service.Client.Manager.API;
using OpenBots.Service.Client.Manager.Execution;
using OpenBots.Service.Client.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBots.Service.Client
{
    public class WindowsServiceEndPoint : IWindowsServiceEndPoint
    {

        public ServerResponse ConnectToServer(ServerConnectionSettings settings)
        {
            // User validation check
            if (!ServiceController.Instance.IsValidUser(settings.UserName))
                return InvalidUserResponse();

            return HttpServerClient.Instance.Connect(settings);
        }

        public ServerResponse DisconnectFromServer(ServerConnectionSettings settings)
        {
            // User validation check
            if (!ServiceController.Instance.IsValidUser(settings.UserName))
                return InvalidUserResponse();

            return HttpServerClient.Instance.Disconnect(settings);
        }

        public async Task<bool> ExecuteAttendedTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            var task = Task.Factory.StartNew(() =>
            {
                // User validation check
                if (!ServiceController.Instance.IsValidUser(settings.UserName))
                    return false;

                return AttendedExecutionManager.Instance.ExecuteTask(projectPackage, settings, isServerAutomation);
            });
            return await task.ConfigureAwait(false);
        }

        public List<string> GetAutomations(string userName)
        {
            // User validation check
            if (!ServiceController.Instance.IsValidUser(userName))
                return null;

            var automationsList = AutomationsAPIManager.GetAutomations(AuthAPIManager.Instance);
            var automationPackageNames = automationsList.Items.Where(
                a => !string.IsNullOrEmpty(a.OriginalPackageName) &&
                a.OriginalPackageName.EndsWith(".nupkg") &&
                a.AutomationEngine.Equals("OpenBots")
                ).Select(a => a.OriginalPackageName).ToList();
            return automationPackageNames;
        }

        public ServerConnectionSettings GetConnectionSettings(string userName)
        {
            // User validation check
            if (!ServiceController.Instance.IsValidUser(userName))
                return null;

            return ConnectionSettingsManager.Instance?.ConnectionSettings ?? null;
        }

        public bool IsAlive()
        {
            return ServiceController.Instance.IsServiceAlive;
        }

        public bool IsConnected(string userName)
        {
            // User validation check
            if (!ServiceController.Instance.IsValidUser(userName))
                return false;

            return ConnectionSettingsManager.Instance?.ConnectionSettings?.ServerConnectionEnabled ?? false;
        }

        public bool IsEngineBusy(string userName)
        {
            // User validation check
            if (!ServiceController.Instance.IsValidUser(userName))
                return true;

            return ExecutionManager.Instance?.IsEngineBusy ?? false;
        }

        public ServerResponse PingServer(ServerConnectionSettings serverSettings)
        {
            try
            {
                // User validation check
                if (!ServiceController.Instance.IsValidUser(serverSettings.UserName))
                    return InvalidUserResponse();

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

        private ServerResponse InvalidUserResponse()
        {
            return new ServerResponse(null, null, $"Environment variable doesn't exist for the current user.");
        }
    }
}
