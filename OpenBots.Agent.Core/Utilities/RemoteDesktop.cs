using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AxMSTSCLib;
using MSTSCLib;
using OpenBots.Agent.Core.Model.RDP;
using OpenBots.Agent.Core.Enums;

namespace OpenBots.Agent.Core.Utilities
{
    public class RemoteDesktop
    {
        public int LogonErrorCode { get; set; }
        public event EventHandler<RemoteDesktopEventArgs> ConnectionStateChangedEvent;
        private AxMsRdpClient9NotSafeForScripting rdpConnection = null;
        public void CreateRdpConnection(string server, string user, string domain, string password, int desktopWidth, int desktopHeight)
        {
            void ProcessTaskThread()
            {
                var form = new Form();
                form.Load += (sender, args) =>
                {
                    rdpConnection = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
                    form.Controls.Add(rdpConnection);
                    rdpConnection.Server = server;
                    rdpConnection.Domain = domain;
                    rdpConnection.UserName = user;
                    rdpConnection.AdvancedSettings9.ClearTextPassword = password;
                    rdpConnection.AdvancedSettings9.EnableCredSspSupport = true;
                    if (true)
                    {
                        rdpConnection.OnDisconnected += RdpConnectionOnOnDisconnected;
                        rdpConnection.OnLoginComplete += RdpConnectionOnOnLoginComplete;
                        rdpConnection.OnLogonError += RdpConnectionOnOnLogonError;
                    }
                    rdpConnection.DesktopHeight = desktopHeight;
                    rdpConnection.DesktopWidth = desktopWidth;
                    rdpConnection.Connect();
                    rdpConnection.Enabled = false;
                    rdpConnection.Dock = DockStyle.Fill;
                    Application.Run(form);
                };
                form.Show();
            }

            var rdpClientThread = new Thread(ProcessTaskThread) { IsBackground = true };
            rdpClientThread.SetApartmentState(ApartmentState.STA);
            rdpClientThread.Start();
            while (rdpClientThread.IsAlive)
            {
                Task.Delay(500).GetAwaiter().GetResult();
            }
        }

        public void DisconnectRDP()
        {
            if(rdpConnection != null)
                rdpConnection.Disconnect();
        }

        private void RdpConnectionOnOnLogonError(object sender, IMsTscAxEvents_OnLogonErrorEvent e)
        {
            LogonErrorCode = e.lError;
            ConnectionStateChangedEvent.Invoke(this, 
                new RemoteDesktopEventArgs() { ConnectionState = RemoteDesktopState.Errored, ErrorCode = e.lError });
        }
        private void RdpConnectionOnOnLoginComplete(object sender, EventArgs e)
        {
            if (LogonErrorCode == -2)
            {
                ConnectionStateChangedEvent.Invoke(this, 
                    new RemoteDesktopEventArgs() { ConnectionState = RemoteDesktopState.Connected, ErrorCode = 0 });
            }
            ConnectionStateChangedEvent.Invoke(this,
                new RemoteDesktopEventArgs() { ConnectionState = RemoteDesktopState.Connected, ErrorCode = 0 });
        }
        private void RdpConnectionOnOnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {

            string disconnectReason = rdpConnection.GetErrorDescription((uint)e.discReason, (uint)rdpConnection.ExtendedDisconnectReason);
            ConnectionStateChangedEvent.Invoke(this, new RemoteDesktopEventArgs() { 
                ConnectionState = RemoteDesktopState.Disconnected, 
                ErrorCode = e.discReason, 
                ErrorDescription = disconnectReason });
        }
    }
}
