namespace OpenBots.Agent.Core.Enums
{
    public enum JobStatus
    {
        New,
        InProgress,
        Complete,
        Fail
    }

    // Sink Type for Logging
    public enum SinkType
    {
        File,
        Http,
        SignalR
    }
}
