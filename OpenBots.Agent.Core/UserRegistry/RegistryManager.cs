using Microsoft.Win32;
using OpenBots.Agent.Core.Utilities;
using System;

namespace OpenBots.Agent.Core.UserRegistry
{
    public class RegistryManager
    {
        private RegistryKeys _registryKeys;

        public RegistryManager()
        {
            _registryKeys = new RegistryKeys();
        }

        public string AgentUsername
        {
            get
            {
                return GetKeyValue(_registryKeys.UsernameKey);
            }
            set
            {
                SetKeyValue(_registryKeys.UsernameKey, value);
            }
        }

        public string AgentPassword
        {
            get
            {
                string password = GetKeyValue(_registryKeys.PasswordKey);
                return string.IsNullOrEmpty(password) ? "" : DataFormatter.DecryptText(password);
            }
            set
            {
                SetKeyValue(_registryKeys.PasswordKey, DataFormatter.EncryptText(value));
            }
        }

        public string ServerURL
        {
            get
            {
                return GetKeyValue(_registryKeys.ServerURLKey);
            }
            set
            {
                SetKeyValue(_registryKeys.ServerURLKey, value);
            }
        }

        private string GetKeyValue(string key)
        {
            string keyValue = null;
            var registryKey = Registry.CurrentUser.OpenSubKey(_registryKeys.SubKey, true);

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

        private void SetKeyValue(string key, string value)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey(_registryKeys.SubKey, true);

            try
            {
                if (registryKey == null)
                    registryKey = Registry.CurrentUser.CreateSubKey(_registryKeys.SubKey);

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
