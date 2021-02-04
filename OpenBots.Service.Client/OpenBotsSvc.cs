using OpenBots.Service.Client.Manager.Agents;
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
            ServiceController.StartService();
        }

        protected override void OnStop()
        {
            ServiceController.StopService();
            AgentsManager.UninitializeAgents();
        }
    }
}
