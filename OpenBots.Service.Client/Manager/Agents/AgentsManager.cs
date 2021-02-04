using System.Collections.Generic;
using System.Linq;

namespace OpenBots.Service.Client.Manager.Agents
{
    public static class AgentsManager
    {
        private static Dictionary<string, Agent> _agentsDict = new Dictionary<string, Agent>();

        public static void AddAgent(string userName)
        {
            if (!_agentsDict.ContainsKey(userName))
                _agentsDict.Add(userName, new Agent());
        }

        public static Agent GetAgent(string userName)
        {
            if (_agentsDict.ContainsKey(userName))
                return _agentsDict[userName];

            return null;
        }

        public static void RemoveAgent(string userName)
        {
            if (_agentsDict.ContainsKey(userName))
                _agentsDict.Remove(userName);
        }

        public static void UninitializeAgents()
        {
            if (_agentsDict.Count > 0)
            {
                for (int index = 0; index < _agentsDict.Count; index++)
                {
                    _agentsDict.ElementAt(index).Value.StopServerCommunication();
                    _agentsDict.Remove(_agentsDict.ElementAt(index).Key);
                }
            }
        }
    }
}
