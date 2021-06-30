namespace OpenBots.Server.SDK.Model
{
    public class DocumentStatus
    {
        public string Status { get; set; }

        public bool IsDocumentCompleted { get; set; }

        public bool HasError { get; set; }

        public bool IsCurrentlyProcessing { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
