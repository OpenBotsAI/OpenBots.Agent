namespace OpenBots.Agent.Client
{
    public class OpenBotsSettings
    {
        public string TracingLevel { get; set; }
        public string SinkType { get; set; }
        public string LoggingValue1 { get; set; }
        public string OpenBotsServerUrl { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public int HeartbeatInterval { get; set; }
        public int JobsLoggingInterval { get; set; }
        public bool HighDensityAgent { get; set; }
        public bool SingleSessionExecution { get; set; }
        public bool SSLCertificateVerification { get; set; }
    }
}
