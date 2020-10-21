using System.Windows;

namespace OpenBots.Agent.Client.Forms.Dialog
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public MessageDialog(string windowTitle, string message)
        {
            InitializeComponent();

            // Set Control Values
            Title = windowTitle;
            txtBlock_Message.Text = message;
        }
    }
}
