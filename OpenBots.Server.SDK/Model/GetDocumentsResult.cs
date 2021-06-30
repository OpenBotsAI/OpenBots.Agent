using System.Collections.Generic;

namespace OpenBots.Server.SDK.Model
{
    public class GetDocumentsResult : DocumentsResult
    {
        public List<ExtractedDocumentView> Result { get; set; }
    }
}
