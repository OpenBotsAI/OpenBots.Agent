namespace OpenBots.Server.SDK.Model
{
    public class UserInfo
    {
        public string ServerType { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ServerUrl { get; set; }
        public string LoginUrl { get; set; }
        public string DocumentsUrl { get; set; }
        public string ApiVersion { get; set; }
        public string UserId { get; set; }
        public string Environment { get; set; }
        public string AgentId { get; set; }
    }
}