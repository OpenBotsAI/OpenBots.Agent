using System;
using System.Runtime.InteropServices;

namespace OpenBots.Service.Client.Manager.Win32
{
    public static class WTSAPI32
    {
        #region Enumerations
        public enum WTS_INFO_CLASS
        {
            WTSInitialProgram,
            WTSApplicationName,
            WTSWorkingDirectory,
            WTSOEMId,
            WTSSessionId,
            WTSUserName,
            WTSWinStationName,
            WTSDomainName,
            WTSConnectState,
            WTSClientBuildNumber,
            WTSClientName,
            WTSClientDirectory,
            WTSClientProductId,
            WTSClientHardwareId,
            WTSClientAddress,
            WTSClientDisplay,
            WTSClientProtocolType,
            WTSIdleTime,
            WTSLogonTime,
            WTSIncomingBytes,
            WTSOutgoingBytes,
            WTSIncomingFrames,
            WTSOutgoingFrames,
            WTSClientInfo,
            WTSSessionInfo,
        }

        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        #endregion

        #region Constants

        public const uint INVALID_SESSION_ID = 0xFFFFFFFF;

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public readonly UInt32 SessionID;

            [MarshalAs(UnmanagedType.LPStr)]
            public readonly String pWinStationName;

            public readonly WTS_CONNECTSTATE_CLASS State;
        }

        #endregion

        #region Win32 API Imports

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern int WTSEnumerateSessions(
            IntPtr hServer,
            int Reserved,
            int Version,
            ref IntPtr ppSessionInfo,
            ref int pCount);

        [DllImport("wtsapi32.dll")]
        public static extern bool WTSQuerySessionInformation(
            IntPtr hServer,
            uint sessionId,
            WTS_INFO_CLASS wtsInfoClass,
            out System.IntPtr ppBuffer,
            out int pBytesReturned);

        [DllImport("wtsapi32.dll")]
        public static extern uint WTSQueryUserToken(uint SessionId, ref IntPtr phToken);

        [DllImport("wtsapi32.dll")]
        public static extern void WTSFreeMemory(IntPtr pointer);

        #endregion
    }
}
