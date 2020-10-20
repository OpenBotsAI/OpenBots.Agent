using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OpenBots.Agent.Client.Forms.Dialog
{
    /// <summary>
    /// Interaction logic for MachineInfo.xaml
    /// </summary>
    public partial class MachineInfo : Window
    {
        private DispatcherTimer _dispatcherTimer;
        public MachineInfo(string machineName, string macAddress, string ipAddress)
        {
            InitializeComponent();
            lbl_MachineInfo_MachineName.Content = machineName;
            lbl_MachineInfo_MACAddress.Content = macAddress;
            lbl_MachineInfo_IPAddress.Content = ipAddress;

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

        private void OnClick_CopyBtn(object sender, RoutedEventArgs e)
        {
            string machineInfo = $"Machine Name: {lbl_MachineInfo_MachineName.Content}\n" +
                $"MAC Address: {lbl_MachineInfo_MACAddress.Content}\n" +
                $"IP Address: {lbl_MachineInfo_IPAddress.Content}\n";

            Clipboard.SetText(machineInfo);

            lbl_CopytoClipboard.Visibility = Visibility.Visible;

            _dispatcherTimer.Start();
        }
    }
}
