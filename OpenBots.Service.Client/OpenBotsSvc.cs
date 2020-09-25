using OpenBots.Service.Client.Server;
using OpenBots.Service.Client.Executor;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace OpenBots.Service.Client
{
    public partial class OpenBotsSvc : ServiceBase
    {
        //Timer _heartbeatTimer = new Timer();
        string scriptPath = @"C:\Users\HP\Documents\taskt\My Scripts\test scripts\ServiceTest\Main.json";
        public OpenBotsSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //_heartbeatTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            //_heartbeatTimer.Interval = 60000; //number in miliseconds  
            //_heartbeatTimer.Enabled = true;

            HttpServerClient.Instance.Initialize();
            ServiceController.Instance.ServiceStart();
        }

        protected override void OnStop()
        {
            ServiceController.Instance.ServiceStop();
            HttpServerClient.Instance.UnInitialize();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{scriptPath}\"";
            // launch the application
            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, out procInfo);
        }
    }
}
