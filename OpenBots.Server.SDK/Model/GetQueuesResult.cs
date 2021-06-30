using System.Collections.Generic;

namespace OpenBots.Server.SDK.Model
{
    public class GetQueuesResult : DocumentsResult
    {
        public Dictionary<string, string> Result { get; set; }
    }
}
