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
            return HttpServerClient.Instance.Connect(settings);
        }

        public ServerResponse DisconnectFromServer(ServerConnectionSettings settings)
        {
            return HttpServerClient.Instance.Disconnect(settings);
        }

        public async Task<bool> ExecuteAttendedTask(string projectPackage, ServerConnectionSettings settings, bool isServerAutomation)
        {
            var task = Task.Factory.StartNew(()=>
            {
                return AttendedExecutionManager.Instance.ExecuteTask(projectPackage, settings, isServerAutomation);
            });
            return await task.ConfigureAwait(false);
        }

        public List<string> GetAutomations()
        {
            var automationsList = AutomationsAPIManager.GetAutomations(AuthAPIManager.Instance);
            var automationPackageNames = automationsList.Items.Where(
                a => !string.IsNullOrEmpty(a.OriginalPackageName) &&
                a.OriginalPackageName.EndsWith(".nupkg") &&
                a.AutomationEngine.Equals("OpenBots")
                ).Select(a => a.OriginalPackageName).ToList();
            return automationPackageNames;
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

        public bool IsEngineBusy()
        {
            return ExecutionManager.Instance?.IsEngineBusy ?? false;
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
