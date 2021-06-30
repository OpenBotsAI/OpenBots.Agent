using System;

namespace OpenBots.Server.SDK.Model
{
    public class DocumentView
    {
        public Guid TaskID { get; set; }

        public ExtractedDocumentView Header { get; set; }

        public DocumentContentView Content { get; set; }
    }
}
