using OpenBots.Agent.Core.Model;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace OpenBots.Agent.Core.Infrastructure
{
    [ServiceContract]
    public interface IWindowsServiceEndPoint
    {
        [OperationContract]
        bool AddAgent(string domainName, string userName);

        [OperationContract]
        [ServiceKnownType(typeof(ServerConnectionSettings))]
        ServerResponse ConnectToServer(ServerConnectionSettings settings);

        [OperationContract]
        [ServiceKnownType(typeof(ServerConnectionSettings))]
        ServerResponse DisconnectFromServer(ServerConnectionSettings settings);

        [OperationContract]
        bool IsConnected(string domainName, string userName);

        [OperationContract]
        bool IsAlive();

        [OperationContract]
        ServerConnectionSettings GetConnectionSettings(string domainName, string userName);

        [OperationContract]
        ServerResponse PingServer(ServerConnectionSettings settings);

        [OperationContract]
        bool IsEngineBusy(string domainName, string userName);

        [OperationContract]
        Task<bool> ExecuteAttendedTask(string projectPath, ServerConnectionSettings settings, bool isServerAutomation);

        [OperationContract]
        ServerResponse GetAutomations(string domainName, string userName);
    }
}
