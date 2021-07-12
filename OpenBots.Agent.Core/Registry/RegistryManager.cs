using Microsoft.Win32;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Utilities;
using System;

namespace OpenBots.Agent.Core.WinRegistry
{
    public class RegistryManager
    {
        private ConfigurationKeys _configurationKeys;

        public RegistryManager()
        {
            _configurationKeys = new ConfigurationKeys();
        }

        public string AgentUsername
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.UsernameKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.UsernameKey, value);
            }
        }

        public string AgentPassword
        {
            get
            {
                string password = GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.PasswordKey);
                return string.IsNullOrEmpty(password) ? "" : DataFormatter.DecryptText(password, SystemInfo.GetMacAddress());
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.PasswordKey, DataFormatter.EncryptText(value, SystemInfo.GetMacAddress()));
            }
        }

        public string ServerURL
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.ServerURLKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.ServerURLKey, value);
            }
        }
        public string AgentOrganization
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.OrganizationKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.OrganizationKey, value);
            }
        }
        public string AgentOrchestrator
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.OrchestratorKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.OrchestratorKey, value);
            }
        }
        public string AgentLogStorage
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogStorageKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogStorageKey, value);
            }
        }
        public string AgentLogLevel
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogLevelKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogLevelKey, value);
            }
        }
        public string AgentLogSink
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogSinkKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogSinkKey, value);
            }
        }
        public string AgentLogHttpURL
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogHttpURLKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogHttpURLKey, value);
            }
        }
        public string AgentLogFilePath
        {
            get
            {
                return GetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogFilePathKey);
            }
            set
            {
                SetKeyValue(RegistryType.User, _configurationKeys.SubKey, _configurationKeys.LogFilePathKey, value);
            }
        }

        public string GetKeyValue(RegistryType registryType, string subKey, string key)
        {
            string keyValue = null;
            var regView = (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            var regHive = (registryType == RegistryType.Machine ? RegistryHive.LocalMachine : RegistryHive.CurrentUser);
            var baseKey = RegistryKey.OpenBaseKey(regHive, regView);
            var registryKey = baseKey.OpenSubKey(subKey, false);

            try
            {
                if (registryKey != null)
                    keyValue = registryKey.GetValue(key)?.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                registryKey?.Close();
            }

            return keyValue;
        }

        public void SetKeyValue(RegistryType registryType, string subKey, string key, string value)
        {
            var regView = (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            var regHive = (registryType == RegistryType.Machine ? RegistryHive.LocalMachine : RegistryHive.CurrentUser);
            var baseKey = RegistryKey.OpenBaseKey(regHive, regView);
            var registryKey = baseKey.OpenSubKey(subKey, true);

            try
            {
                if (registryKey == null)
                    registryKey = baseKey.CreateSubKey(subKey);

                registryKey.SetValue(key, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                registryKey?.Close();
            }
        }
    }
}
