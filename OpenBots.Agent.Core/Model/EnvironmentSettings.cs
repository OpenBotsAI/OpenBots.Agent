using Microsoft.Win32;
using System;
using System.IO;
using System.Security.Principal;

namespace OpenBots.Agent.Core.Model
{
    public class EnvironmentSettings
    {
        public string EnvironmentVariableName { get; } = "OpenBots_Agent_Data_Path";
        public string CloudServerEnvironmentVariable { get; } = "OpenBots_Environment";
        public string EnvironmentVariablePath { get; } = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "OpenBots Inc",
                        "OpenBots Agent"
                        );
        public string SettingsFileName { get; } = "OpenBots.settings";
        public string EnvironmentVariableValue { get; private set; }
        public EnvironmentSettings()
        {
        }

        public string GetEnvironmentVariablePath(string domainName = "", string userName = "")
        {
            try
            {
                if(!string.IsNullOrEmpty(userName))
                    return EnvironmentVariableExists(domainName, userName) ? EnvironmentVariableValue : string.Empty;

                return Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.User);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void SetEnvironmentVariablePath()
        {
            try
            {
                Environment.SetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariablePath, EnvironmentVariableTarget.User);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // To check if the set environment variable path is valid for the current user
        public bool isValidEnvironmentVariable()
        {
            return EnvironmentVariablePath.Equals(GetEnvironmentVariablePath());
        }

        // To check if the required environment variable exists for the given User Name
        public bool EnvironmentVariableExists(string domainName, string userName)
        {
            try
            {
                var envVariable = GetUserEnvironmentVariable(domainName, userName, EnvironmentVariableName);
                if(envVariable != null)
                {
                    EnvironmentVariableValue = envVariable.ToString();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object GetUserEnvironmentVariable(string domainName, string userName, string variableName)
        {
            try
            {
                NTAccount f = new NTAccount(domainName, userName);
                SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
                string sidString = s.ToString();

                var regView = (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.Users, regView);
                var baseKeyPath = Path.Combine(sidString, "Environment");
                var environmentKey = baseKey.OpenSubKey(baseKeyPath);
                var envVariable = environmentKey.GetValue(variableName);

                return envVariable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetPathEnvironmentVariable(string domainName, string userName)
        {
            try
            {
                NTAccount f = new NTAccount(domainName, userName);
                SecurityIdentifier s = (SecurityIdentifier)f.Translate(typeof(SecurityIdentifier));
                string sidString = s.ToString();

                var regView = (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.Users, regView);
                var baseKeyPath = Path.Combine(sidString, "Environment");
                var environmentKey = baseKey.OpenSubKey(baseKeyPath);
                var variableNames = environmentKey.GetValueNames();

                string pathVariableVal = string.Empty;
                foreach(var varName in variableNames)
                {
                    if(varName.ToLower().Equals("path"))
                    {
                        pathVariableVal = environmentKey.GetValue(varName).ToString();
                        break;
                    }
                }

                return pathVariableVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetCloudServerEnvironmentVariable()
        {
            try
            {
                var envVariable = Environment.GetEnvironmentVariable(CloudServerEnvironmentVariable, EnvironmentVariableTarget.Machine);

                if (!string.IsNullOrEmpty(envVariable))
                    return envVariable.ToString();
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
