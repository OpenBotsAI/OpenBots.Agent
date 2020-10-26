using Newtonsoft.Json;
using OpenBots.Agent.Core.Model;
using System;
using System.IO;

namespace OpenBots.Agent.Client
{
    public class SettingsManager
    {
        public EnvironmentSettings EnvironmentSettings;
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
            EnvironmentSettings = new EnvironmentSettings();
        }

        public void UpdateSettings(OpenBotsSettings agentSettings)
        {
            File.WriteAllText(GetSettingsFilePath(), JsonConvert.SerializeObject(agentSettings, Formatting.Indented));
        }

        public OpenBotsSettings ReadSettings()
        {
            return JsonConvert.DeserializeObject<OpenBotsSettings>(File.ReadAllText(GetSettingsFilePath()));
        }

        public OpenBotsSettings GetDefaultSettings()
        {
            // Default Settings
            return new OpenBotsSettings()
            {
                TracingLevel = "Information",
                SinkType = "Http",
                LoggingValue1 = "/api/v1/Logger/Agent",
                LoggingValue2 = "",
                LoggingValue3 = "",
                LoggingValue4 = "",
                OpenBotsServerUrl = "",
                AgentId = "",
                AgentName = ""
            };
        }

        public OpenBotsSettings ResetToDefaultSettings()
        {
            var agentSettings = GetDefaultSettings();
            agentSettings.LoggingValue1 = string.Empty;

            UpdateSettings(agentSettings);

            return agentSettings;
        }

        public string GetSettingsFilePath()
        {
            return Path.Combine(EnvironmentSettings.EnvironmentVariableValue, EnvironmentSettings.SettingsFileName);
        }
    }
}
