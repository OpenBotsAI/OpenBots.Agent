using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenBots.Agent.Client.Forms.Dialog
{
    /// <summary>
    /// Interaction logic for MachineInfo.xaml
    /// </summary>
    public partial class MachineInfo : Window
    {
        private DispatcherTimer _dispatcherTimer;
        public MachineInfo(string whoami, string machineName, string macAddress, string ipAddress, string serverGeneratedIP)
        {
            InitializeComponent();
            lbl_MachineInfo_WhoAmI.Content = whoami;
            lbl_MachineInfo_MachineName.Content = machineName;
            lbl_MachineInfo_MACAddress.Content = macAddress;
            lbl_MachineInfo_IPAddress.Content = ipAddress;
            lbl_MachineInfo_ServerIPAddress.Content = string.IsNullOrEmpty(serverGeneratedIP) ? "No URL Provided" : serverGeneratedIP;
            lbl_MachineInfo_ServerIPAddress.Foreground = string.IsNullOrEmpty(serverGeneratedIP) ? Brushes.Red : Brushes.Black;

            //Create a timer with interval of 2 secs
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            lbl_CopytoClipboard.Visibility = Visibility.Collapsed;

            //Disable the timer
            _dispatcherTimer.IsEnabled = false;
        }

        private void DisplayCopiedMessage()
        {
            lbl_CopytoClipboard.Visibility = Visibility.Visible;

            _dispatcherTimer.Start();
        }

        private void OnClick_CopyImage(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string imageName = sender.GetType().GetProperty("Name").GetValue(sender, null).ToString();

            switch (imageName)
            {
                case "img_whoami":
                    Clipboard.SetText(lbl_MachineInfo_WhoAmI.Content.ToString());
                    break;
                case "img_machineName":
                    Clipboard.SetText(lbl_MachineInfo_MachineName.Content.ToString());
                    break;
                case "img_macAddress":
                    Clipboard.SetText(lbl_MachineInfo_MACAddress.Content.ToString());
                    break;
                case "img_ipAddress":
                    Clipboard.SetText(lbl_MachineInfo_IPAddress.Content.ToString());
                    break;
                case "img_serverIPAddress":
                    Clipboard.SetText(lbl_MachineInfo_ServerIPAddress.Content.ToString());
                    break;
            }
            DisplayCopiedMessage();
        }
    }
}
