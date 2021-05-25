using OpenBots.Agent.Core.Enums;
using System;

namespace OpenBots.Agent.Core.Model.RDP
{
    public class RemoteDesktopEventArgs: EventArgs
    {
        public RemoteDesktopState ConnectionState { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}
