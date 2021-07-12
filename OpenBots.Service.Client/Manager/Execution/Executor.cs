using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Model.RDP;
using OpenBots.Agent.Core.WinRegistry;
using OpenBots.Service.Client.Manager.FreeRDP;
using OpenBots.Service.Client.Manager.Logs;
using OpenBots.Service.Client.Manager.Win32;
using Serilog.Events;
using System;
using System.Threading;
using static OpenBots.Service.Client.Manager.Win32.WTSAPI32;

namespace OpenBots.Service.Client.Manager.Execution
{
    /// <summary>
    /// Class that allows running applications with full admin rights. In
    /// addition the application launched will bypass the Vista UAC prompt.
    /// </summary>
    public class Executor
    {
        private Win32Utilities _win32Helper;
        private FileLogger _fileLogger;

        public Executor(FileLogger fileLogger)
        {
            _win32Helper = new Win32Utilities();
            _fileLogger = fileLogger;
        }

        /// <summary>
        /// Launches given application in the security context of specified user
        /// </summary>
        /// <param name="commandLine">Command line containing automation info to be run by the executor</param>
        /// <param name="machineCredential">Machine credentials contaning the User Account info to run the job for.</param>
        /// <param name="serverSettings">Server Connection Settings containing screen resolution info for RDP session.</param>
        public void RunAutomation(String commandLine, MachineCredential machineCredential, ServerConnectionSettings serverSettings)
        {
            bool isRDPSession = false;
            IntPtr hPToken = IntPtr.Zero;
            RDPSessionManager rdpSessionManager = null;
            string domainUsername = $"{machineCredential.Domain}\\{machineCredential.UserName}";
            RDPRegistryKeys rdpRegistryKeys = new RDPRegistryKeys();
            RegistryManager registryManager = new RegistryManager();

            try
            {
                // Obtain the currently active session id for given User Credentials
                bool sessionFound = _win32Helper.GetUserSessionToken(machineCredential, ref hPToken);

                _fileLogger?.LogEvent("GetActiveUserSession", "Existing active user session " +
                    (!sessionFound ? "not found" : "found"), LogEventLevel.Information);

                // If unable to find an Active User Session (for given User Credentials)
                if (!sessionFound)
                {
                    _fileLogger?.LogEvent("ValidateUser", "Validate User Account Credentials", LogEventLevel.Information);

                    int errorCode;
                    if (!_win32Helper.ValidateUser(machineCredential, out errorCode))
                        throw new Exception($"Unable to Create an Active User Session " +
                            $"as the provided User Credentials are invalid.\n" +
                            (errorCode != 0 ? $"Error Code: {errorCode}" : string.Empty));

                    // Bypassing Legal Notice Screen (if Enabled)
                    GetLegalNoticeValues(rdpRegistryKeys, registryManager);
                    SetLegalNoticeValues(rdpRegistryKeys, registryManager, false);

                    _fileLogger?.LogEvent("CreateRDPConnection", "Start RDP Connection", LogEventLevel.Information);

                    try
                    {
                        rdpSessionManager = new RDPSessionManager();
                        rdpSessionManager.OpenRDPSession(new RemoteDesktopInfo
                        {
                            Host = "localhost",
                            User = machineCredential.UserName,
                            Domain = machineCredential.Domain,
                            Password = machineCredential.PasswordSecret,
                            DesktopWidth = serverSettings.ResolutionWidth,
                            DesktopHeight = serverSettings.ResolutionHeight,
                            ColorDepth = 32
                        });

                        if (!rdpSessionManager.isConnected)
                        {
                            _fileLogger?.LogEvent("CreateRDPConnection", "Unable to create the RDP Session", LogEventLevel.Error);
                            throw new Exception($"Unable to Create a Remote Desktop Session for provided Credential \"{machineCredential.Name}\" ");

                            //_fileLogger?.LogEvent("LogonUser", "Logon User to perform automation in non-interactive session", LogEventLevel.Error);

                            //if (!(sessionFound = _win32Helper.LogonUserA(machineCredential, ref hPToken)))
                            //    throw new Exception($"Unable to Create an Active User Session for provided Credential \"{machineCredential.Name}\" ");
                        }
                        else
                        {
                            _fileLogger?.LogEvent("CreateRDPConnection", "Connecting to RDP Connection", LogEventLevel.Information);
                            _fileLogger?.LogEvent("CreateRDPConnection", "Wait for RDP Connection", LogEventLevel.Information);

                            // Wait for RDP connection to be established within 60 sec
                            bool isConnected = WaitForRDPConnection(domainUsername);

                            // Obtain the id of Remote Desktop Session
                            sessionFound = _win32Helper.GetUserSessionToken(machineCredential, ref hPToken);

                            _fileLogger?.LogEvent("GetActiveUserSession", "Session " +
                                (!sessionFound ? "not found" : "found"), LogEventLevel.Information);

                            // If unable to find/create an Active User Session
                            if (!sessionFound)
                                throw new Exception($"Unable to Create a Remote Desktop Session for provided Credential \"{machineCredential.Name}\" ");

                            isRDPSession = rdpSessionManager.isConnected;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        SetLegalNoticeValues(rdpRegistryKeys, registryManager, true);
                    }
                }
                _fileLogger?.LogEvent("RunProcessAsCurrentUser", $"Start Automation", LogEventLevel.Information);

                // Start a new process in the current user's logon session
                _win32Helper.RunProcessAsCurrentUser(hPToken, commandLine);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    if (hPToken != IntPtr.Zero)
                        _win32Helper.ClosePtrHandle(hPToken);

                    if (isRDPSession)
                        rdpSessionManager.CloseRDPSession();
                }
                catch (Exception)
                {
                    // Suppress Exception
                }
            }
        }

        private bool WaitForRDPConnection(string domainUser, int seconds = 60)
        {
            bool isConnected = true;

            int sec = 0;
            while (sec < seconds)
            {
                if (_win32Helper.GetActiveUserSessionId(domainUser) != INVALID_SESSION_ID)
                    break;

                Thread.Sleep(1000);
                sec++;
            }

            if (sec == 60)
                isConnected = false;

            return isConnected;
        }

        private void GetLegalNoticeValues(RDPRegistryKeys rdpRegistryKeys, RegistryManager registryManager)
        {
            try
            {
                rdpRegistryKeys.LegalNoticeCaptionValue = registryManager.GetKeyValue(RegistryType.Machine,
                                rdpRegistryKeys.SubKey, rdpRegistryKeys.LegalNoticeCaptionKey);

                rdpRegistryKeys.LegalNoticeTextValue = registryManager.GetKeyValue(RegistryType.Machine,
                    rdpRegistryKeys.SubKey, rdpRegistryKeys.LegalNoticeTextKey);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred while getting the Legal-Notice Registry Values: {ex.Message}");
            }
        }

        private void SetLegalNoticeValues(RDPRegistryKeys rdpRegistryKeys, RegistryManager registryManager, bool setBack)
        {
            try
            {
                if (!string.IsNullOrEmpty(rdpRegistryKeys.LegalNoticeCaptionValue) ||
                        !string.IsNullOrEmpty(rdpRegistryKeys.LegalNoticeTextValue))
                {
                    registryManager.SetKeyValue(RegistryType.Machine, rdpRegistryKeys.SubKey, rdpRegistryKeys.LegalNoticeCaptionKey,
                            (!string.IsNullOrEmpty(rdpRegistryKeys.LegalNoticeCaptionValue) && !setBack) ? "" : rdpRegistryKeys.LegalNoticeCaptionValue);

                    registryManager.SetKeyValue(RegistryType.Machine, rdpRegistryKeys.SubKey, rdpRegistryKeys.LegalNoticeTextKey,
                        (!string.IsNullOrEmpty(rdpRegistryKeys.LegalNoticeTextValue) && !setBack) ? "" : rdpRegistryKeys.LegalNoticeTextValue);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception occurred while setting the Legal-Notice Registry Values: {ex.Message}");
            }
        }
    }
}
