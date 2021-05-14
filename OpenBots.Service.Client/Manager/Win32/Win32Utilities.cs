using OpenBots.Agent.Core.Model;
using System;
using System.Runtime.InteropServices;
using static OpenBots.Service.Client.Manager.Win32.ADVAPI32;
using static OpenBots.Service.Client.Manager.Win32.WTSAPI32;
using static OpenBots.Service.Client.Manager.Win32.KERNEL32;
using static OpenBots.Service.Client.Manager.Win32.USERENV32;
using System.DirectoryServices.AccountManagement;

namespace OpenBots.Service.Client.Manager.Win32
{
    public class Win32Utilities
    {
        private readonly IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        public Win32Utilities()
        {

        }

        public string GetUsernameBySessionId(uint sessionId, bool prependDomain)
        {
            IntPtr buffer;
            int strLen;
            string username = string.Empty;
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSUserName, out buffer, out strLen) && strLen > 1)
            {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if (prependDomain)
                {
                    if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WTS_INFO_CLASS.WTSDomainName, out buffer, out strLen) && strLen > 1)
                    {
                        username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                        WTSFreeMemory(buffer);
                    }
                }
            }
            return username;
        }

        public uint GetActiveUserSessionId(string domainUsername)
        {
            var activeSessionId = INVALID_SESSION_ID;
            var pSessionInfo = IntPtr.Zero;
            var sessionCount = 0;

            if (WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref pSessionInfo, ref sessionCount) != 0)
            {
                var arrayElementSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
                var current = pSessionInfo;

                for (var i = 0; i < sessionCount; i++)
                {
                    // Get Session Info
                    var si = (WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += arrayElementSize;

                    // Get Domain\Username by Session Id
                    var sessionUsername = GetUsernameBySessionId(si.SessionID, true);

                    // If there is an Active Session for the Assigned User
                    if (si.State == WTS_CONNECTSTATE_CLASS.WTSActive && sessionUsername.ToLower() == domainUsername.ToLower())
                    {
                        activeSessionId = si.SessionID;
                        break;
                    }
                }
            }

            return activeSessionId;
        }

        public bool GetUserSessionToken(MachineCredential machineCredential, ref IntPtr phUserToken)
        {
            var bResult = false;
            var hImpersonationToken = IntPtr.Zero;
            string domainUsername = $"{machineCredential.Domain}\\{machineCredential.UserName}";

            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.nLength = (uint)Marshal.SizeOf(sa);

            // Get Session Id of the active session.
            var activeSessionId = GetActiveUserSessionId(domainUsername);

            // If activeSessionId is valid (Active Session found for given User Credentials)
            if (activeSessionId != INVALID_SESSION_ID &&
                WTSQueryUserToken(activeSessionId, ref hImpersonationToken) != 0)
            {
                // Convert the impersonation token to a primary token
                bResult = DuplicateTokenEx(hImpersonationToken, 0, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
                    (int)TOKEN_TYPE.TokenPrimary, ref phUserToken);

                CloseHandle(hImpersonationToken);
            }

            return bResult;
        }

        public bool LogonUserA(MachineCredential machineCredential, ref IntPtr phUserToken)
        {
            return LogonUser(
                machineCredential.UserName,
                machineCredential.Domain,
                machineCredential.PasswordSecret,
                LOGON32_LOGON_BATCH,
                LOGON32_PROVIDER_DEFAULT,
                ref phUserToken);
        }

        public bool ValidateUser(MachineCredential machineCredential, out int errorCode)
        {
            bool isValid = false;
            errorCode = 0;

            // Validate using PrincipalContext (Directory Services) | Method-1
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Machine, Environment.MachineName))
                {
                    isValid = pc.ValidateCredentials(machineCredential.UserName, machineCredential.PasswordSecret);
                }
            }
            catch (Exception)
            {
                // do nothing
            }

            // Validate using LogonUser Method (ADVAPI32.dll) | Method-2    (Only if Method-1 fails)
            if (!isValid)
            {
                IntPtr phUserToken = new IntPtr();
                try
                {
                    if (!(isValid = LogonUser(machineCredential.UserName, machineCredential.Domain,
                        machineCredential.PasswordSecret, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT,
                        ref phUserToken)))
                    {
                        errorCode = Marshal.GetLastWin32Error();
                    }
                }
                catch (Exception)
                {
                    // do nothing
                }
                finally
                {
                    CloseHandle(phUserToken);
                }
            }
            return isValid;
        }

        public bool RunProcessAsCurrentUser(IntPtr hPToken, string commandLine)
        {
            uint exitCode = 0;
            bool pResult = false;
            IntPtr envBlock = IntPtr.Zero;
            UInt32 pResultWait = WAIT_FAILED;
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();

            try
            {
                SECURITY_ATTRIBUTES securityAttribute = new SECURITY_ATTRIBUTES();
                securityAttribute.nLength = (uint)Marshal.SizeOf(securityAttribute);

                // Setting lpDesktop =  "winsta0\default" enable the process to run in an interactive window station and desktop
                STARTUPINFO startupInfo = new STARTUPINFO();
                startupInfo.cb = (int)Marshal.SizeOf(startupInfo);
                startupInfo.lpDesktop = @"winsta0\default";

                // flags that specify the priority and creation method of the process
                int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE | CREATE_UNICODE_ENVIRONMENT;

                // Create Environment Block
                if (!CreateEnvironmentBlock(ref envBlock, hPToken, false))
                {
                    throw new Exception($"Failure: Could not create environment block.\n Error: {Marshal.GetLastWin32Error()}");
                }

                // create a new process in the current user's logon session
                pResult = CreateProcessAsUser(hPToken,                  // client's access token
                                                null,                   // file to execute
                                                commandLine,            // command line
                                                ref securityAttribute,  // pointer to process SECURITY_ATTRIBUTES
                                                ref securityAttribute,  // pointer to thread SECURITY_ATTRIBUTES
                                                false,                  // handles are not inheritable
                                                dwCreationFlags,        // creation flags
                                                envBlock,               // pointer to new environment block 
                                                null,                   // name of current directory 
                                                ref startupInfo,        // pointer to STARTUPINFO structure
                                                out processInfo         // receives information about new process
                                                );

                if (!pResult)
                    throw new Exception($"Failure: Could not create process as user.\n Error: {Marshal.GetLastWin32Error()}");

                pResultWait = WaitForSingleObject(processInfo.hProcess, INFINITE);
                if (pResultWait == WAIT_FAILED)
                    throw new Exception($"Failure: Could not wait for process to complete.\n Error: {Marshal.GetLastWin32Error()}");

                GetExitCodeProcess(processInfo.hProcess, out exitCode);

                if (exitCode != 0)
                    throw new Exception($"Exception occurred in the Executor App when starting automation.\n Exit Code: {exitCode}");
            }
            finally
            {
                // invalidate the handles
                CloseHandle(processInfo.hProcess);
                CloseHandle(processInfo.hThread);
                CloseHandle(hPToken);

                if (envBlock != IntPtr.Zero)
                {
                    DestroyEnvironmentBlock(envBlock);
                }
            }

            return pResult;
        }

        public void ClosePtrHandle(IntPtr ptrHandle)
        {
            CloseHandle(ptrHandle);
        }
    }
}
