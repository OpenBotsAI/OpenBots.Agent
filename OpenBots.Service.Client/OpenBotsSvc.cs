using OpenBots.Service.Client.Server;
using OpenBots.Service.Client.Manager.Execution;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace OpenBots.Service.Client
{
    public partial class OpenBotsSvc : ServiceBase
    {
        //////Timer _heartbeatTimer = new Timer();
        //////string scriptPath = @"C:\Users\HP\Documents\OpenBotsStudio\My Scripts\RunJobTest\Main.json";
        public OpenBotsSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            HttpServerClient.Instance.Initialize();
            ServiceController.Instance.StartService();
            
            //////_heartbeatTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            //////_heartbeatTimer.Interval = 10000; //number in miliseconds  
            //////_heartbeatTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            ServiceController.Instance.StopService();
            HttpServerClient.Instance.UnInitialize();

            //////_heartbeatTimer.Elapsed -= new ElapsedEventHandler(OnElapsedTime);
            //////_heartbeatTimer.Enabled = false;
        }

        //////private void OnElapsedTime(object source, ElapsedEventArgs e)
        //////{
        //////    _heartbeatTimer.Elapsed -= new ElapsedEventHandler(OnElapsedTime);
        //////    _heartbeatTimer.Enabled = false;

        //////    var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
        //////    var cmdLine = $"\"{executorPath}\" \"{scriptPath}\"";
        //////    // launch the application
        //////    ProcessLauncher.PROCESS_INFORMATION procInfo;
        //////    ProcessLauncher.LaunchProcess(cmdLine, out procInfo);
        //////}
    }
}
