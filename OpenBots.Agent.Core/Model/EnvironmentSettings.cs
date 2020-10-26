using System;
using System.IO;

namespace OpenBots.Agent.Core.Model
{
    public class EnvironmentSettings
    {
        public string EnvironmentVariableName { get; } = "OpenBots_Agent_Data_Path";
        public string EnvironmentVariableValue { get; } = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "OpenBots Inc",
                        "OpenBots Agent"
                        );
        public string SettingsFileName { get; } = "OpenBots.settings";

        public EnvironmentSettings()
        {
        }

        public string GetEnvironmentVariable()
        {
            return Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.Machine);
        }
    }
}
