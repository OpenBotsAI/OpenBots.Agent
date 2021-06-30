namespace OpenBots.Agent.Core.Model.RDP
{
    public class RemoteDesktopInfo
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Domain { get; set; }
        public string Password { get; set; }
        public int DesktopWidth { get; set; }
        public int DesktopHeight { get; set; }
        public int ColorDepth { get; set; }
    }
}
