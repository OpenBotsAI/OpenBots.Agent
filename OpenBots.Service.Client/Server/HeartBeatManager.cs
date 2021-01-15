using OpenBots.Agent.Core.Enums;
using OpenBots.Service.API.Model;
using System;

namespace OpenBots.Service.Client.Server
{
    public class HeartBeatManager
    {
        public AgentHeartbeat Heartbeat { get; set; }
        public static HeartBeatManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new HeartBeatManager();

                return instance;
            }
        }
        private static HeartBeatManager instance;

        private HeartBeatManager()
        {
            Heartbeat = new AgentHeartbeat();
        }

        public void Initialize(string agentId)
        {
            Heartbeat.AgentId = new Guid(agentId);
            Heartbeat.LastReportedStatus = AgentStatus.Available.ToString();
            Heartbeat.LastReportedWork = string.Empty;
            Heartbeat.LastReportedMessage = string.Empty;
            Heartbeat.IsHealthy = true;
        }
    }
}
