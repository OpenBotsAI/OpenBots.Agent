using Newtonsoft.Json;
using System;
using System.IO;

namespace OpenBots.Agent.Client
{
    public class SettingsManager
    {
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SettingsManager();

                return instance;
            }
        }
        private static SettingsManager instance;

        private SettingsManager()
        {
        }

        public string EnvironmentVariableName { get; } = "OpenBots_Agent_Config_Path";
        public string EnvironmentVariableValue { get; } = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "OpenBots Inc",
                        "OpenBots Agent",
                        "OpenBots.settings"
                        );

        public void UpdateSettings(OpenBotsSettings agentSettings)
        {
            File.WriteAllText(GetSettingsFilePath(), JsonConvert.SerializeObject(agentSettings, Formatting.Indented));
        }

        public OpenBotsSettings ReadSettings()
        {
            return JsonConvert.DeserializeObject<OpenBotsSettings>(File.ReadAllText(GetSettingsFilePath()));
        }

        public OpenBotsSettings ResetToDefaultSettings()
        {
            // Default Settings
            var agentSettings = new OpenBotsSettings()
            {
                TracingLevel = "Information",
                SinkType = "Http",
                LoggingValue1 = "https://openbotsserver-dev.azurewebsites.net/api/v1/Logger/Agent",
                LoggingValue2 = "",
                LoggingValue3 = "",
                LoggingValue4 = "",
                OpenBotsServerUrl = "",
                AgentId = "",
                AgentName = ""
            };

            UpdateSettings(agentSettings);

            return agentSettings;
        }

        private string GetSettingsFilePath()
        {
            var settingsFilePath = Environment.GetEnvironmentVariable(
                        SettingsManager.Instance.EnvironmentVariableName,
                        EnvironmentVariableTarget.Machine);

            return settingsFilePath ?? EnvironmentVariableValue;
        }
    }
}
