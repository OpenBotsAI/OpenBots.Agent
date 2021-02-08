using OpenBots.Agent.Client.Settings;
using OpenBots.Agent.Core.Infrastructure;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ServiceModel;

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

        public bool AddAgent()
        {
            return _pipeProxy.AddAgent(SystemInfo.GetUserDomainName(), Environment.UserName);
        }

        public ServerResponse ConnectToServer()
        {
            return _pipeProxy.ConnectToServer(ConnectionSettingsManager.Instance.ConnectionSettings);
        }

        public ServerResponse DisconnectFromServer()
        {
            return _pipeProxy.DisconnectFromServer(ConnectionSettingsManager.Instance.ConnectionSettings);
        }

        public bool IsServiceAlive()
        {
            return _pipeProxy.IsAlive();
        }

        public bool IsServerConnectionUp()
        {
            try
            {
                return _pipeProxy.IsConnected(SystemInfo.GetUserDomainName(), Environment.UserName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ServerConnectionSettings GetConnectionHistory()
        {
            return _pipeProxy.GetConnectionSettings(SystemInfo.GetUserDomainName(), Environment.UserName);
        }

        public ServerResponse PingServer()
        {
            return _pipeProxy.PingServer(ConnectionSettingsManager.Instance.ConnectionSettings);
        }

        public async void ExecuteAttendedTask(string projectPackagePath, ServerConnectionSettings settings, bool isServerAutomation = false)
        {
            try
            {
                var task = _pipeProxy.ExecuteAttendedTask(projectPackagePath, settings, isServerAutomation);
                await task.ContinueWith(e => TaskFinishedEvent?.Invoke(this, task.Result));
            }
            catch (TimeoutException ex)
            {
                throw ex;
            }
        }

        public List<string> GetAutomations()
        {
            return _pipeProxy.GetAutomations(SystemInfo.GetUserDomainName(), Environment.UserName);
        }

        public bool IsEngineBusy()
        {
            return _pipeProxy.IsEngineBusy(SystemInfo.GetUserDomainName(), Environment.UserName);
        }
    }
}
