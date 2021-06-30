using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace OpenBots.Server.SDK.Model
{
    public class DocumentInfo
    {
        public DocumentInfo()
        {
            Documents = new List<DocumentHeader>();
        }

        public string TaskId { get; set; }
        public List<DocumentHeader> Documents { get; set; }

        public void Add(int documentNumber, string schema = "", string pageNumbers = "", string folder = "", string documentId = "")
        {
            Documents.Add(new DocumentHeader(documentNumber, schema, pageNumbers, folder, documentId));
        }
        public DataTable CreateDataTable()
        {
            DataTable table = new DataTable("Results");
            table.Columns.Add("DocumentNumber", typeof(int));
            table.Columns.Add("Schema", typeof(string));
            table.Columns.Add("PageNumbers", typeof(string));
            table.Columns.Add("Folder", typeof(string));
            table.Columns.Add("DocumentId", typeof(string));
            table.Columns.Add("Confidence", typeof(double));

            if (Documents != null)
                foreach (DocumentHeader header in Documents)
                    if (header != null)
                        header.AddRow(table);

            return table;
        }

        public string SerializeJSON()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
