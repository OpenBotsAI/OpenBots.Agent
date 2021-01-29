using OpenBots.Agent.Core.Model;
using System;
using System.ServiceModel;

namespace OpenBots.Service.Client
{
    public class ServiceController
    {
        private ServiceHost _serviceHost;
        private EnvironmentSettings _environmentSettings;
        public static ServiceController Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServiceController();

                return instance;
            }
        }
        private static ServiceController instance;

        private ServiceController()
        {
            _environmentSettings = new EnvironmentSettings();
        }

        public Boolean IsServiceAlive
        {
            get
            {
                return _serviceHost != null && _serviceHost.State == CommunicationState.Opened;
            }
        }

        public void StopService()
        {
            if (IsServiceAlive)
            {
                _serviceHost.Close();
                _serviceHost = null;
            }
            else if (_serviceHost != null)
            {
                _serviceHost = null;
            }
        }
        public void StartService()
        {
            StopService();
            _serviceHost = new ServiceHost(typeof(WindowsServiceEndPoint));
            _serviceHost.Open();
        }

        public bool IsValidUser(string domainName, string userName)
        {
            return _environmentSettings.EnvironmentVariableExists(domainName, userName);
        }
    }
}
