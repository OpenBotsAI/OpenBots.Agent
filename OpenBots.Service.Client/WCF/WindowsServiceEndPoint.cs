using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using OpenBots.Service.Client.Manager.Agents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenBots.Service.Client
{
    public class WindowsServiceEndPoint : IWindowsServiceEndPoint
    {
        public bool AddAgent(string domainName, string userName)
        {
            // User validation check
            if (!ServiceController.IsValidUser(domainName, userName))
                return false;

            AgentsManager.AddAgent(userName);
            return true;
        }

        public ServerResponse ConnectToServer(ServerConnectionSettings settings)
        {
            // User validation check
            if (!ServiceController.IsValidUser(settings.DNSHost, settings.UserName))
                return InvalidUserResponse();

            return AgentsManager.GetAgent(settings.UserName).Connect(settings);
        }

        public ServerResponse DisconnectFromServer(ServerConnectionSettings settings)
        {
            // User validation check
            if (!ServiceController.IsValidUser(settings.DNSHost, settings.UserName))
                return InvalidUserResponse();

            return AgentsManager.GetAgent(settings.UserName).Disconnect(settings);
        }

        public async Task<bool> ExecuteAttendedTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            var task = Task.Factory.StartNew(() =>
            {
                // User validation check
                if (!ServiceController.IsValidUser(settings.DNSHost, settings.UserName))
                    return false;

                return AgentsManager.GetAgent(settings.UserName).ExecuteAttendedTask(projectPackage, settings, isServerAutomation);
            });
            return await task.ConfigureAwait(false);
        }

        public ServerResponse GetAutomations(string domainName, string userName, string automationEngine)
        {
            // User validation check
            if (!ServiceController.IsValidUser(domainName, userName))
                return null;

            return AgentsManager.GetAgent(userName).GetAutomations(automationEngine);
        }

        public ServerConnectionSettings GetConnectionSettings(string domainName, string userName)
        {
            // User validation check
            if (!ServiceController.IsValidUser(domainName, userName))
                return null;

            return AgentsManager.GetAgent(userName).GetConnectionSettings();
        }

        public bool IsAlive()
        {
            return ServiceController.IsServiceAlive();
        }

        public bool IsConnected(string domainName, string userName)
        {
            // User validation check
            if (!ServiceController.IsValidUser(domainName, userName))
                return false;

            return AgentsManager.GetAgent(userName).IsConnectedToServer();
        }

        public bool IsEngineBusy(string domainName, string userName)
        {
            // User validation check
            if (!ServiceController.IsValidUser(domainName, userName))
                return true;

            return AgentsManager.GetAgent(userName).IsEngineBusy();
        }

        public ServerResponse PingServer(ServerConnectionSettings serverSettings)
        {
            // User validation check
            if (!ServiceController.IsValidUser(serverSettings.DNSHost, serverSettings.UserName))
                return InvalidUserResponse();

            return AgentsManager.GetAgent(serverSettings.UserName).PingServer(serverSettings);
        }

        private ServerResponse InvalidUserResponse()
        {
            return new ServerResponse(null, null, $"Environment variable doesn't exist for the current user.");
        }
    }
}
