using OpenBots.Service.Client.Server;
using OpenBots.Service.Client.Manager.Execution;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using OpenBots.Agent.Core.Model;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using OpenBots.Service.API.Model;

namespace OpenBots.Service.Client
{
    public partial class OpenBotsSvc : ServiceBase
    {
        Timer _heartbeatTimer = new Timer();
        string mainScriptFilePath = @"C:\RunJobTest\UnattendedTest\Main.json";
        string processName = "FileFolderTest";
        public OpenBotsSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //HttpServerClient.Instance.Initialize();
            //ServiceController.Instance.StartService();

            _heartbeatTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            _heartbeatTimer.Interval = 20000; //number in miliseconds  
            _heartbeatTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            //ServiceController.Instance.StopService();
            //HttpServerClient.Instance.UnInitialize();

            _heartbeatTimer.Elapsed -= new ElapsedEventHandler(OnElapsedTime);
            _heartbeatTimer.Enabled = false;
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            _heartbeatTimer.Elapsed -= new ElapsedEventHandler(OnElapsedTime);
            _heartbeatTimer.Enabled = false;

            //////var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            //////var cmdLine = $"\"{executorPath}\" \"{scriptPath}\"";
            //////// launch the application
            //////ProcessLauncher.PROCESS_INFORMATION procInfo;
            //////ProcessLauncher.LaunchProcess(cmdLine, out procInfo);

            var executionParams = GetExecutionParams("", "", processName, mainScriptFilePath);
            var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
            var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";

            Credential machineCredential = new Credential(null, null, null, "AliTest", "AccelirateAdmin", "WeLoveRobots123", null);

            // launch the Executor
            ProcessLauncher.PROCESS_INFORMATION procInfo;
            ProcessLauncher.LaunchProcess(cmdLine, machineCredential, out procInfo);
        }

        private string GetExecutionParams(string jobId, string processId, string processName, string mainScriptFilePath)
        {
            var executionParams = new JobExecutionParams()
            {
                JobId = jobId,
                ProcessId = processId,
                ProcessName = processName,
                MainFilePath = mainScriptFilePath,
                ProjectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath),
                ServerConnectionSettings = new ServerConnectionSettings()
                {
                    TracingLevel = "Information",
                    SinkType = "File",
                    LoggingValue1 = @"C:\RunJobTest\Logs.txt"
                }
            };
            var paramsJsonString = JsonConvert.SerializeObject(executionParams);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(paramsJsonString));
        }
    }
}
