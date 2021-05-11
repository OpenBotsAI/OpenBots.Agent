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

    public enum OrchestratorType
    {
        Cloud,
        Local
    }

    // Sink Type for Logging
    public enum SinkType
    {
        File,
        Http
    }

    public enum AgentStatus
    {
        Available,
        Busy
    }

    // Source of Automations
    public enum AutomationSource
    {
        Local,
        Server
    }

    // Boolean Alias
    public enum BooleanAlias
    {
        Yes,
        No
    }

    // Types of Automations
    public enum AutomationType
    {
        OpenBots,
        Python,
        TagUI,
        CSScript
    }

    public enum RemoteDesktopState
    {
        Connected,
        Disconnected,
        Errored,
        Unknown
    }
}
