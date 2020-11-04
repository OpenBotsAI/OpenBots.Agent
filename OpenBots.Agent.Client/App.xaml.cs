using OpenBots.Agent.Client.Utilities;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace OpenBots.Agent.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Get this Process
            Process currProcess = Process.GetCurrentProcess();

            // Get Count of All App Instances of Agent
            var agentInstances = Process.GetProcesses().Where(p =>
                p.ProcessName == currProcess.ProcessName &&
                p.SessionId == currProcess.SessionId);

            // For more than one instances
            if (agentInstances.Count() > 1)
            {
                // Close Current App
                App.Current.Shutdown();

                // Bring existing one to front
                WindowHelper.BringWindowToFront();
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    }
}
