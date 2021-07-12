using Newtonsoft.Json;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using System;
using System.IO;

namespace OpenBots.Agent.Client
{
    public class SettingsManager
    {
        public EnvironmentSettings EnvironmentSettings;
        public string LogAPIEndPoint = "/api/v1/Logger/Agent";
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
                AgentId = "",
                AgentName = "",
                HeartbeatInterval = 30,
                JobsLoggingInterval = 5,
                ResolutionWidth = 0,
                ResolutionHeight = 0,
                HighDensityAgent = false,
                SingleSessionExecution = false,
                SSLCertificateVerification = false,
                ServerConfigurationSource = ServerConfigurationSource.Registry.ToString()
            };
        }

        public OpenBotsSettings ResetToDefaultSettings()
        {
            var agentSettings = GetDefaultSettings();
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
