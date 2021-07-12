using System.Runtime.Serialization;

namespace OpenBots.Agent.Core.Model
{
    [DataContract]
    public class ServerConnectionSettings
    {
        [DataMember]
        public string ServerType { get; set; }
        [DataMember]
        public string ServerURL { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public string AgentUsername { get; set; }
        [DataMember]
        public string AgentPassword { get; set; }
        [DataMember]
        public string DNSHost { get; set; }
        [DataMember]
        public string UserName { get; set; }
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
        public string ServerIPAddress { get; set; }
        [DataMember]
        public string TracingLevel { get; set; }
        [DataMember]
        public string LogStorage { get; set; }
        [DataMember]
        public string SinkType { get; set; }
        [DataMember]
        public string LogUrl { get; set; }
        [DataMember]
        public string LogFilePath { get; set; }
        [DataMember]
        public int HeartbeatInterval { get; set; }
        [DataMember]
        public int JobsLoggingInterval { get; set; }
        [DataMember]
        public int ResolutionWidth { get; set; }
        [DataMember]
        public int ResolutionHeight { get; set; }
        [DataMember]
        public bool HighDensityAgent { get; set; }
        [DataMember]
        public bool SingleSessionExecution { get; set; }
        [DataMember]
        public bool SSLCertificateVerification { get; set; }
        [DataMember]
        public bool ServerConnectionEnabled { get; set; }
    }
}
