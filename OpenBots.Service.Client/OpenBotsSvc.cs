using OpenBots.Service.Client.Server;
using OpenBots.Service.Client.Manager.Execution;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using OpenBots.Agent.Core.Model;
using System.Text.Json;

namespace OpenBots.Service.Client
{
    public partial class OpenBotsSvc : ServiceBase
    {
        //////Timer _heartbeatTimer = new Timer();
        //////string mainScriptFilePath = @"D:\Projects\RPA\Taskt\Git\OpenBots\OpenBots.Agent\OpenBots.Service.Client\bin\Debug\Processes\RunJobTest1\Main.json";
        //////string processName = "RunJobTest1";
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

        //////    //////var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
        //////    //////var cmdLine = $"\"{executorPath}\" \"{scriptPath}\"";
        //////    //////// launch the application
        //////    //////ProcessLauncher.PROCESS_INFORMATION procInfo;
        //////    //////ProcessLauncher.LaunchProcess(cmdLine, out procInfo);

        //////    var executionParams = GetExecutionParams(processName, mainScriptFilePath);
        //////    var executorPath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "OpenBots.Executor.exe").FirstOrDefault();
        //////    var cmdLine = $"\"{executorPath}\" \"{executionParams}\"";

        //////    // launch the Executor
        //////    ProcessLauncher.PROCESS_INFORMATION procInfo;
        //////    ProcessLauncher.LaunchProcess(cmdLine, out procInfo);
        //////}

        //////private string GetExecutionParams(string processName, string mainScriptFilePath)
        //////{
        //////    var executionParams = new JobExecutionParams()
        //////    {
        //////        ProcessName = processName,
        //////        MainFilePath = mainScriptFilePath,
        //////        ProjectDirectoryPath = Path.GetDirectoryName(mainScriptFilePath),
        //////        ServerConnectionSettings = ConnectionSettingsManager.Instance.ConnectionSettings
        //////    };

        //////    return JsonSerializer.Serialize(executionParams);
        //////}
    }
}
