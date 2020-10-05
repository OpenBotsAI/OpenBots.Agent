using System.Dynamic;

namespace OpenBots.Agent.Core.UserRegistry
{
    public class RegistryKeys
    {
        public string SubKey { get; } = @"Environment\OpenBots\Agent\Credentials";
        public string UsernameKey { get; } = "Username";
        public string PasswordKey { get; } = "Password";
    }
}
