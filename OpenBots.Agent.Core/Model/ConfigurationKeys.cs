
namespace OpenBots.Agent.Core.Model
{
    public class ConfigurationKeys
    {
        public string SubKey { get; } = @"SOFTWARE\OpenBots\Agent\Credentials";
        public string UsernameKey { get; } = "OPENBOTS_USERNAME";
        public string PasswordKey { get; } = "OPENBOTS_EPASSWORD";
        public string ServerURLKey { get; } = "OPENBOTS_HOSTNAME";
        public string OrganizationKey { get; } = "OPENBOTS_ORGANIZATION";
        public string OrchestratorKey { get; } = "OPENBOTS_ORCHESTRATOR";
        public string ProvisionKey { get; } = "OPENBOTS_PROVISIONKEY";
        public string LogStorageKey { get; } = "OPENBOTS_LOGSTORE";
        public string LogLevelKey { get; } = "OPENBOTS_LOG_LEVEL";
        public string LogSinkKey { get; } = "OPENBOTS_LOG_SINK";
        public string LogHttpURLKey { get; } = "OPENBOTS_LOG_HTTPURL";
        public string LogFilePathKey { get; } = "OPENBOTS_LOG_FILEPATH";
    }
}
