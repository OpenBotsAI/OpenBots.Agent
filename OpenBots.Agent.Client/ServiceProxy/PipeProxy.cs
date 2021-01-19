using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace OpenBots.Agent.Client
{
    public class PipeProxy
    {
        IWindowsServiceEndPoint _pipeProxy;
        public event EventHandler<bool> TaskFinishedEvent;

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
                // Create NamedPipe Binding and Set SendTimeout
                var namedPipeBinding = new NetNamedPipeBinding();
                namedPipeBinding.SendTimeout = new TimeSpan(24, 0, 0);

                ChannelFactory<IWindowsServiceEndPoint> pipeFactory = new ChannelFactory<IWindowsServiceEndPoint>(
                        namedPipeBinding,
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

        public ServerResponse PingServer(ServerConnectionSettings connectionSettings)
        {
            return _pipeProxy.PingServer(connectionSettings);
        }

        public async void ExecuteAttendedTask(string projectPackagePath, ServerConnectionSettings settings)
        {
            try
            {
                var task = _pipeProxy.ExecuteAttendedTask(projectPackagePath, settings);
                await task.ContinueWith(e => TaskFinishedEvent?.Invoke(this, task.Result));
            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
        }

        public bool IsEngineBusy()
        {
            return _pipeProxy.IsEngineBusy();
        }
    }
}
