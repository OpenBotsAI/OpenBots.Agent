using Microsoft.Win32;
using OpenBots.Server.SDK.Model;
using System;
using System.IO;
using System.Security.Principal;

namespace OpenBots.Server.SDK.HelperMethods
{
    public static class UtilityMethods
    {
        public static string GetUserEnvironmentVariable(string domainName, string userName, string variableName)
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

                return envVariable?.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void RefreshToken(UserInfo userInfo)
        {
            var authMethods = new AuthMethods();
            authMethods.RefreshToken(userInfo);
        }
    }
}