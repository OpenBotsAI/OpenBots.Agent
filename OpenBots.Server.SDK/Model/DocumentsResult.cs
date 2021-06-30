namespace OpenBots.Server.SDK.Model
{
    public class DocumentsResult
    {
        public string TargetUrl { get; set; }
        public bool? Success { get; set; }
        public string Error { get; set; }
        public bool? UnAuthorizedRequest { get; set; }
        public bool? __abp { get; set; }
    }
}
