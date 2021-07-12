using OpenBots.Agent.Core.Model.RDP;
using System;
using System.Runtime.InteropServices;

namespace OpenBots.Service.Client.Manager.FreeRDP
{
    public class RDPSessionManager
    {
        public bool isConnected { get; private set; }
        private const string _freerdpLibPath = "freerdp-client3.dll";

        private RdpConnectResponse _rdpConnectResponse;

        [StructLayout(LayoutKind.Sequential)]
        public struct RdpConnectRequest
        {

            [MarshalAs(UnmanagedType.LPStr)]
            public string host;
            [MarshalAs(UnmanagedType.LPStr)]
            public string user;
            [MarshalAs(UnmanagedType.LPStr)]
            public string domain;
            [MarshalAs(UnmanagedType.LPStr)]
            public string password;
            public int desktopWidth;
            public int desktopHeight;
            public int colorDepth;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct RdpConnectResponse
        {
            public int status;
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr context;
        };

        // Used by Agent for Connection
        [StructLayout(LayoutKind.Sequential)]
        public struct RdpDisconnectResponse
        {
            public int status;
            [MarshalAs(UnmanagedType.SysUInt)]
            public IntPtr context;
        };

        [DllImport(_freerdpLibPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void FreeRDPConnect(RdpConnectRequest connectRequest, [MarshalAs(UnmanagedType.Struct)] ref RdpConnectResponse connectResponse);
        [DllImport(_freerdpLibPath, CallingConvention = CallingConvention.Cdecl)]
        static extern void FreeRDPDisconnect(RdpConnectResponse disconnectRequest, [MarshalAs(UnmanagedType.Struct)] ref RdpDisconnectResponse disconnectResponse);

        public RDPSessionManager()
        {
        }

        public void OpenRDPSession(RemoteDesktopInfo rdpInfo)
        {
            _rdpConnectResponse = new RdpConnectResponse();
            RdpConnectRequest connectRequest = new RdpConnectRequest();
            connectRequest.host = rdpInfo.Host;
            connectRequest.user = rdpInfo.User;
            connectRequest.domain = rdpInfo.Domain;
            connectRequest.password = rdpInfo.Password;
            connectRequest.desktopWidth = rdpInfo.DesktopWidth;
            connectRequest.desktopHeight = rdpInfo.DesktopHeight;
            connectRequest.colorDepth = rdpInfo.ColorDepth;

            FreeRDPConnect(connectRequest, ref _rdpConnectResponse);

            isConnected = _rdpConnectResponse.status == 1 ? true : false;
        }

        public void CloseRDPSession()
        {
            RdpDisconnectResponse disconnectResponse = new RdpDisconnectResponse();

            if(isConnected)
                FreeRDPDisconnect(_rdpConnectResponse, ref disconnectResponse);

            isConnected = disconnectResponse.status == 1 ? false : true;
        }
    }
}
