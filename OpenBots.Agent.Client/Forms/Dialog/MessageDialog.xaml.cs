using System.Windows;

namespace OpenBots.Agent.Client.Forms.Dialog
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public bool CloseManually { get; set; }
        public MessageDialog(string windowTitle, string message, bool closeManually)
        {
            InitializeComponent();

            // Set Control Values
            Title = windowTitle;
            txtBlock_Message.Text = message;
            CloseManually = closeManually;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized && !CloseManually)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
        }
    }
}
