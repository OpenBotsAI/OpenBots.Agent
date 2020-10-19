using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using System;
using System.ServiceModel;

namespace OpenBots.Agent.Client
{
    public class PipeProxy
    {
        IWindowsServiceEndPoint _pipeProxy;
        public static PipeProxy Instance
        {
            get
            {
                if (instance == null)
                    instance = new PipeProxy();

                return instance;
            }
        }
        private static PipeProxy instance;

        private PipeProxy()
        {
        }

        public bool StartServiceEndPoint()
        {
            try
            {
                ChannelFactory<IWindowsServiceEndPoint> pipeFactory =
                      new ChannelFactory<IWindowsServiceEndPoint>(
                        new NetNamedPipeBinding(),
                        new EndpointAddress("net.pipe://localhost/OpenBots/WindowsServiceEndPoint"));

                _pipeProxy = pipeFactory.CreateChannel();
                return IsServiceAlive();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ServerResponse ConnectToServer(ServerConnectionSettings connectionSettings)
        {
            return _pipeProxy.ConnectToServer(connectionSettings);
        }

        public ServerResponse DisconnectFromServer(ServerConnectionSettings connectionSettings)
        {
            return _pipeProxy.DisconnectFromServer(connectionSettings);
        }

        public bool IsServiceAlive()
        {
            return _pipeProxy.IsAlive();
        }

        public bool IsServerConnectionUp()
        {
            try
            {
                return _pipeProxy.IsConnected();
            }
            catch (Exception)
            {

                return false;
            }
        }

        public ServerConnectionSettings GetConnectionHistory()
        {
            return _pipeProxy.GetConnectionSettings();
        }

        public void SetConfigFilePath(string environmentVariable, string settingsFilePath)
        {
            _pipeProxy.SetEnvironmentVariable(environmentVariable, settingsFilePath);
        }
    }
}
