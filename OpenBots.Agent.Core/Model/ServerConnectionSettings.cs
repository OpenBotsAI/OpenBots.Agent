using System.Runtime.Serialization;

namespace OpenBots.Agent.Core.Model
{
    [DataContract]
    public class ServerConnectionSettings
    {
        [DataMember]
        public string ServerURL { get; set; }
        [DataMember]
        public string AgentUsername { get; set; }
        [DataMember]
        public string AgentPassword { get; set; }
        [DataMember]
        public string DNSHost { get; set; }
        [DataMember]
        public string WhoAmI { get; set; }
        [DataMember]
        public string MachineName { get; set; }
        [DataMember]
        public string AgentId { get; set; }
        [DataMember]
        public string AgentName { get; set; }
        [DataMember]
        public string MACAddress { get; set; }
        [DataMember]
        public string IPAddress { get; set; }
        [DataMember]
        public string ServerIPAddress { get; set; }
        [DataMember]
        public string TracingLevel { get; set; }
        [DataMember]
        public string SinkType { get; set; }
        [DataMember]
        public string LoggingValue1 { get; set; }
        [DataMember]
        public string LoggingValue2 { get; set; }
        [DataMember]
        public string LoggingValue3 { get; set; }
        [DataMember]
        public string LoggingValue4 { get; set; }
        [DataMember]
        public bool ServerConnectionEnabled { get; set; }
    }
}
