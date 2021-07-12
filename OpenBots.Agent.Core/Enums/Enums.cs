namespace OpenBots.Agent.Core.Enums
{
    public enum OrchestratorType
    {
        Cloud,
        Local
    }

    public enum ServerConfigurationSource
    {
        Environment,
        Registry
    }

    // Sink Type for Logging
    public enum SinkType
    {
        Http,
        File
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

    public enum RegistryType
    {
        Machine,
        User
    }
}
