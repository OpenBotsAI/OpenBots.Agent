using System.ServiceModel;
using OpenBots.Agent.Core.Model;

namespace OpenBots.Agent.Core.Infrastructure
{
    [ServiceContract]
    public interface IWindowsServiceEndPoint
    {
        [OperationContract]
        [ServiceKnownType(typeof(ServerConnectionSettings))]
        ServerResponse ConnectToServer(ServerConnectionSettings settings);

        [OperationContract]
        [ServiceKnownType(typeof(ServerConnectionSettings))]
        ServerResponse DisconnectFromServer(ServerConnectionSettings settings);

        [OperationContract]
        bool IsConnected();

        [OperationContract]
        bool IsAlive();

        [OperationContract]
        ServerConnectionSettings GetConnectionSettings();

        [OperationContract]
        void SetEnvironmentVariable(string environmentVariable, string settingsFilePath);

        [OperationContract]
        ServerResponse PingServer(ServerConnectionSettings settings);
    }
}
