using System;
using System.Runtime.InteropServices;

namespace OpenBots.Service.Client.Manager.Win32
{
    public static class ADVAPI32
    {
        #region Enumerations

        public enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        public enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }

        #endregion

        #region Constants

        public const int IDLE_PRIORITY_CLASS = 0x40;
        public const int NORMAL_PRIORITY_CLASS = 0x20;
        public const int HIGH_PRIORITY_CLASS = 0x80;
        public const int REALTIME_PRIORITY_CLASS = 0x100;

        public const int TOKEN_DUPLICATE = 0x0002;
        public const uint MAXIMUM_ALLOWED = 0x2000000;
        public const int CREATE_NEW_CONSOLE = 0x00000010;
        public const int CREATE_NO_WINDOW = 0x08000000;
        public const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_FAILED = 0xFFFFFFFF;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        #endregion

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

        #region Win32 API Imports

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string username, string domain, string password, 
            int logonType, int logonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, 
            ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, 
            int dwCreationFlags, IntPtr lpEnvironment, String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, 
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
            int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

        #endregion
    }
}
