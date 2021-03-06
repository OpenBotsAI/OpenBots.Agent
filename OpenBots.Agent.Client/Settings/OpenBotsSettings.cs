﻿namespace OpenBots.Agent.Client
{
    public class OpenBotsSettings
    {
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public int HeartbeatInterval { get; set; }
        public int JobsLoggingInterval { get; set; }
        public int ResolutionWidth { get; set; }
        public int ResolutionHeight { get; set; }
        public bool HighDensityAgent { get; set; }
        public bool SingleSessionExecution { get; set; }
        public bool SSLCertificateVerification { get; set; }
        public string ServerConfigurationSource { get; set; }
    }
}
