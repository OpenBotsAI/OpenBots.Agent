using System.Data;

namespace OpenBots.Server.SDK.Model
{
    public class DocumentResult
    {
        public string OutputAsJSON { get; set; }
        public DataTable OutputAsTable { get; set; }
        public DataTable DataAsTable { get; set; }
        public string Status { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasFailedOrError { get; set; }
    }
}
