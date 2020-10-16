using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using System;
using System.ServiceModel;
using System.Windows;

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
            try
            {
                return _pipeProxy.ConnectToServer(connectionSettings);
            }
            catch (Exception Ex)
            {
                ErrorDialog errorDialog = new ErrorDialog("An error occurred while connecting to the server.",
                        Ex.GetType().Name,
                        Ex.Message);
                errorDialog.Owner = Application.Current.MainWindow;
                errorDialog.ShowDialog();

                return null;
            }   
        }

        public ServerResponse DisconnectFromServer(ServerConnectionSettings connectionSettings)
        {
            try
            {
                return _pipeProxy.DisconnectFromServer(connectionSettings);
            }
            catch (Exception Ex)
            {
                ErrorDialog errorDialog = new ErrorDialog("An error occurred while disconnecting from the server.",
                        Ex.Source,
                        Ex.Message);
                errorDialog.Owner = Application.Current.MainWindow;
                errorDialog.ShowDialog();

                return null;
            }
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
    }
}
