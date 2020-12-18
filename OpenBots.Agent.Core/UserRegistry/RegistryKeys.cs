namespace OpenBots.Agent.Core.UserRegistry
{
    public class RegistryKeys
    {
        public string SubKey { get; } = @"SOFTWARE\OpenBots\Agent\Credentials";
        public string UsernameKey { get; } = "Username";
        public string PasswordKey { get; } = "Password";
        public string ServerURLKey { get; } = "ServerURL";
    }
}
