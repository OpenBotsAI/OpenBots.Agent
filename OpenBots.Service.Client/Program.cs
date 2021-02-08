using System.ServiceProcess;

namespace OpenBots.Service.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new OpenBotsSvc()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
