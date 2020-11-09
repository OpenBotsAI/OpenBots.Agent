using OpenBots.Service.Client.Server;
using System.ServiceProcess;

namespace OpenBots.Service.Client
{
    public partial class OpenBotsSvc : ServiceBase
    {
        public OpenBotsSvc()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            HttpServerClient.Instance.Initialize();
            ServiceController.Instance.StartService();
        }

        protected override void OnStop()
        {
            ServiceController.Instance.StopService();
            HttpServerClient.Instance.UnInitialize();
        }
    }
}
