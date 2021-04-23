using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AxMSTSCLib;
using MSTSCLib;

namespace OpenBots.Agent.Core.Utilities
{
    public class RemoteDesktop
    {
        public int LogonErrorCode { get; set; }
        public event EventHandler<int> ConnectionStateChangedEvent;
        private AxMsRdpClient9NotSafeForScripting rdpConnection = null;
        public void CreateRdpConnection(string server, string user, string domain, string password)
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
            ConnectionStateChangedEvent.Invoke(this, 2);
            //File.AppendAllText(@"C:\temp1.txt", $"OnLogonError: {LogonErrorCode} EventTime: {DateTime.Now.ToString()}\n");
        }
        private void RdpConnectionOnOnLoginComplete(object sender, EventArgs e)
        {
            if (LogonErrorCode == -2)
            {
                //File.AppendAllText(@"C:\temp1.txt", $"OnLoginComplete: New Session Detected. EventTime: {DateTime.Now.ToString()}\n");
                ConnectionStateChangedEvent.Invoke(this, 3);
            }
            //File.AppendAllText(@"C:\temp1.txt", $"OnLoginComplete: New Session Created. EventTime: {DateTime.Now.ToString()}\n");
            ConnectionStateChangedEvent.Invoke(this, 1);
        }
        private void RdpConnectionOnOnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            //File.AppendAllText(@"C:\temp1.txt", $"OnDisconnected: New Session Terminated. EventTime: {DateTime.Now.ToString()}\n");
            ConnectionStateChangedEvent.Invoke(this, 0);
        }
    }
}
