using OpenBots.Agent.Core.Model;
using System.ServiceModel;

namespace OpenBots.Service.Client
{
    public static class ServiceController
    {
        private static ServiceHost _serviceHost;
        private static EnvironmentSettings _environmentSettings;

        public static bool IsServiceAlive()
        {
            return _serviceHost != null && _serviceHost.State == CommunicationState.Opened;
        }

        public static void StopService()
        {
            if (IsServiceAlive())
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
            else if (_serviceHost != null)
            {
                _serviceHost = null;
            }
        }
        public static void StartService()
        {
            // Starting WCF Service
            StopService();
            _serviceHost = new ServiceHost(typeof(WindowsServiceEndPoint));
            _serviceHost.Open();

            // Initializing EnvironmentSettings
            _environmentSettings = new EnvironmentSettings();
        }

        public static bool IsValidUser(string domainName, string userName)
        {
            return _environmentSettings.EnvironmentVariableExists(domainName, userName);
        }
    }
}
