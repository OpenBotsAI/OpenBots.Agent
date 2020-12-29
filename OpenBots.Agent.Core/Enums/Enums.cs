using System.ComponentModel;

namespace OpenBots.Agent.Core.Enums
{
    public enum JobStatus
    {
        Unknown,
        New,
        Assigned,
        InProgress,
        Completed,
        Failed,
        Abandoned
    }

    // Sink Type for Logging
    public enum SinkType
    {
        File,
        Http
    }
}
