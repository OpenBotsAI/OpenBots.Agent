using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace OpenBots.Service.Client.Manager.Execution
{
    /// <summary>
    /// Class that allows running applications with full admin rights. In
    /// addition the application launched will bypass the Vista UAC prompt.
    /// </summary>
    public class ProcessLauncher
    {
        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        #endregion

        #region Enumerations

        enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }

        #endregion

        #region Constants

        public const int TOKEN_DUPLICATE = 0x0002;
        public const uint MAXIMUM_ALLOWED = 0x2000000;
        public const int CREATE_NEW_CONSOLE = 0x00000010;
        public const int CREATE_NO_WINDOW = 0x08000000;
        private const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_ASSIGN_PRIMARY = 0x0001;

        public const int IDLE_PRIORITY_CLASS = 0x40;
        public const int NORMAL_PRIORITY_CLASS = 0x20;
        public const int HIGH_PRIORITY_CLASS = 0x80;
        public const int REALTIME_PRIORITY_CLASS = 0x100;

        const UInt32 INFINITE = 0xFFFFFFFF;
        const UInt32 WAIT_FAILED = 0xFFFFFFFF;
        #endregion

        #region Win32 API Imports

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        static extern bool ProcessIdToSessionId(uint dwProcessId, ref uint pSessionId);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
            int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject
        (
            IntPtr hHandle,
            UInt32 dwMilliseconds
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint ExitCode);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(ref IntPtr lpEnvironment, IntPtr hToken, bool bInherit);
        #endregion

        #region Helping Methods
        private static IntPtr GetEnvironmentBlock(IntPtr token)
        {

            IntPtr envBlock = IntPtr.Zero;
            bool retVal = CreateEnvironmentBlock(ref envBlock, token, false);
            if (retVal == false)
            {

                //Environment Block, things like common paths to My Documents etc.
                //Will not be created if "false"
                //It should not adversley affect CreateProcessAsUser.

                throw new Exception(String.Format("CreateEnvironmentBlock Error: {0}", Marshal.GetLastWin32Error()));
            }
            return envBlock;
        }

        private static IntPtr GetPrimaryToken(int processId)
        {
            IntPtr token = IntPtr.Zero;
            IntPtr primaryToken = IntPtr.Zero;
            bool retVal = false;
            Process p = null;

            try
            {
                p = Process.GetProcessById(processId);
            }

            catch (ArgumentException)
            {

                string details = String.Format("ProcessID {0} Not Available", processId);
                Debug.WriteLine(details);
                throw;
            }


            //Gets impersonation token
            retVal = OpenProcessToken(p.Handle, TOKEN_DUPLICATE, ref token);
            if (retVal == true)
            {

                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.nLength = (uint)Marshal.SizeOf(sa);

                //Convert the impersonation token into Primary token
                retVal = DuplicateTokenEx(
                    token,
                    TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_QUERY,
                    ref sa,
                    (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                    (int)TOKEN_TYPE.TokenPrimary,
                    ref primaryToken);

                //Close the Token that was previously opened.
                CloseHandle(token);
                if (retVal == false)
                {
                    string message = String.Format("DuplicateTokenEx Error: {0}", Marshal.GetLastWin32Error());
                    Debug.WriteLine(message);
                }

            }

            else
            {

                string message = String.Format("OpenProcessToken Error: {0}", Marshal.GetLastWin32Error());
                Debug.WriteLine(message);

            }

            //We'll Close this token after it is used.
            return primaryToken;

        }
        #endregion Helping Methods

        /// <summary>
        /// Launches the given application with full admin rights, and in addition bypasses the Vista UAC prompt
        /// </summary>
        /// <param name="commandLine">The name of the application to launch</param>
        /// <param name="processInfo">Process information regarding the launched application that gets returned to the caller</param>
        /// <returns>Exit Code of the Process</returns>
        public static bool LaunchProcess(String commandLine, out PROCESS_INFORMATION processInfo)
        {
            int winlogonPid = 0;
            uint exitCode = 0;
            Boolean pResult = false;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            UInt32 pResultWait = WAIT_FAILED;

            processInfo = new PROCESS_INFORMATION();

            try
            {
                // obtain the currently active session id; every logged on user in the system has a unique session id
                uint dwSessionId = WTSGetActiveConsoleSessionId();

                // obtain the process id of the winlogon process that is running within the currently active session
                Process[] processes = Process.GetProcessesByName("winlogon");
                foreach (Process p in processes)
                {
                    if ((uint)p.SessionId == dwSessionId)
                    {
                        winlogonPid = (int)p.Id;
                    }
                }

                // obtain a handle to the winlogon process
                //hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

                //// obtain a handle to the access token of the winlogon process
                //if (!OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
                //{
                //    CloseHandle(hProcess);
                //    return false;
                //}

                hPToken = GetPrimaryToken(winlogonPid);

                if(hPToken == IntPtr.Zero)
                {
                    throw new Exception("Couldn't retrieve the primary token.");
                }

                // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser
                // I would prefer to not have to use a security attribute variable and to just 
                // simply pass null and inherit (by default) the security attributes
                // of the existing token. However, in C# structures are value types and therefore
                // cannot be assigned the null value.
                SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
                sa.nLength = (uint) Marshal.SizeOf(sa);

                //// copy the access token of the winlogon process; the newly created token will be a primary token
                //if (!DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)TOKEN_TYPE.TokenPrimary, ref hUserTokenDup))
                //{
                //    CloseHandle(hProcess);
                //    CloseHandle(hPToken);
                //    return false;
                //}

                // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning
                // the window station has a desktop that is invisible and the process is incapable of receiving
                // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user 
                // interaction with the new process.
                STARTUPINFO si = new STARTUPINFO();
                si.cb = (int)Marshal.SizeOf(si);
                si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop

                // flags that specify the priority and creation method of the process
                int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE | CREATE_UNICODE_ENVIRONMENT;

                IntPtr envBlock = GetEnvironmentBlock(hPToken);

                // create a new process in the current user's logon session
                pResult = CreateProcessAsUser(hPToken,            // client's access token
                                                null,                   // file to execute
                                                commandLine,            // command line
                                                ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                                                ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                                false,                  // handles are not inheritable
                                                dwCreationFlags,        // creation flags
                                                envBlock,            // pointer to new environment block 
                                                null,                   // name of current directory 
                                                ref si,                 // pointer to STARTUPINFO structure
                                                out processInfo         // receives information about new process
                                                );

                if (!pResult) { throw new Exception("CreateProcessAsUser error #" + Marshal.GetLastWin32Error()); }

                pResultWait = WaitForSingleObject(processInfo.hProcess, INFINITE);
                if (pResultWait == WAIT_FAILED) { throw new Exception("WaitForSingleObject error #" + Marshal.GetLastWin32Error()); }

                GetExitCodeProcess(processInfo.hProcess, out exitCode);
            }
            finally
            {
                // invalidate the handles
                CloseHandle(hProcess);
                CloseHandle(hPToken);
                CloseHandle(hUserTokenDup);
            }

            return pResult; // return the result
        }

    }
}