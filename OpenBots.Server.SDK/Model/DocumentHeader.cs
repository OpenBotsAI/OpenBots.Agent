using System;
using System.Data;

namespace OpenBots.Server.SDK.Model
{
    public class DocumentHeader
    {
        public DocumentHeader()
        {
        }

        public DocumentHeader(int documentNumber, string schema = "", string pageNumbers = "", string folder = "", string documentId = "")
        {
            DocumentNumber = documentNumber;
            Schema = schema;
            PageNumbers = pageNumbers;
            Folder = folder;
            DocumentId = documentId;
            Confidence = 0.9;
        }

        public int DocumentNumber { get; set; }
        public string Schema { get; set; }
        public string PageNumbers { get; set; }

        public string Folder { get; set; }

        public string DocumentId { get; set; }

        public double Confidence { get; set; }



        public void AddRow(DataTable table)
        {
            DataRow row = table.NewRow();
            row["DocumentNumber"] = this.DocumentNumber;
            row["Schema"] = this.Schema;
            row["PageNumbers"] = this.PageNumbers;
            row["Folder"] = this.Folder;
            row["DocumentId"] = this.DocumentId;
            row["Confidence"] = this.Confidence;
            table.Rows.Add(row);
        }
    }
}
