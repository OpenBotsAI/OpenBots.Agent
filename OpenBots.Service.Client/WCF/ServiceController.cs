using System;
using System.ServiceModel;

namespace OpenBots.Service.Client
{
    public class ServiceController
    {
        private ServiceHost serviceHost;
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

        }

        public Boolean IsServiceAlive
        {
            get
            {
                return serviceHost != null && serviceHost.State == CommunicationState.Opened;
            }
        }

        public void StopService()
        {
            if (IsServiceAlive)
            {
                serviceHost.Close();
                serviceHost = null;
            }
            else if (serviceHost != null)
            {
                serviceHost = null;
            }
        }
        public void StartService()
        {
            StopService();
            serviceHost = new ServiceHost(typeof(WindowsServiceEndPoint));
            serviceHost.Open();
        }
    }
}
