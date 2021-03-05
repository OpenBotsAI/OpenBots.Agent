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
                OpenBotsServerUrl = "",
                AgentId = "",
                AgentName = "",
                HeartbeatInterval = 60,
                JobsLoggingInterval = 60,
                HighDensityAgent = false,
                SingleSessionExecution = false,
                SSLCertificateVerification = false
            };
        }

        public OpenBotsSettings ResetToDefaultSettings()
        {
            var agentSettings = GetDefaultSettings();
            agentSettings.LoggingValue1 = string.Empty;

            UpdateSettings(agentSettings);

            return agentSettings;
        }

        public void CreateAgentSettingsFile()
        {
            try
            {
                if (!File.Exists(GetSettingsFilePath()))
                {
                    var agentSettings = GetDefaultSettings();
                    UpdateSettings(agentSettings);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetSettingsFilePath()
        {
            // If "...\OpenBots Inc\OpenBots Agent\" Directory doesn't exist
            if (!Directory.Exists(EnvironmentSettings.EnvironmentVariablePath))
                Directory.CreateDirectory(EnvironmentSettings.EnvironmentVariablePath);

            return Path.Combine(EnvironmentSettings.EnvironmentVariablePath, EnvironmentSettings.SettingsFileName);
        }
    }
}
