using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace OpenBots.Server.SDK.Model
{
    public class ExtractedContentContainer
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, ExtractedContentField>>> ExtractedContent { get; set; }
    }

    public class ExtractedContentDatas
    {
        public Dictionary<string, List<ExtractedContentDocument>> document { get; set; }
    }

    public class ExtractedContentDocument
    {
        public Dictionary<string, ExtractedContentField> fields { get; set; }
    }
    public class ExtractedContentField
    {

        public static Dictionary<string, ExtractedContentField> Parse(string jsonExtractedContent)
        {

            if (!string.IsNullOrEmpty(jsonExtractedContent) && jsonExtractedContent.StartsWith("\"") && jsonExtractedContent.EndsWith("\""))
                jsonExtractedContent = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<object>(jsonExtractedContent));

            JObject o = JObject.Parse(jsonExtractedContent);
            var eCon = o["ExtractedContent"];
            var eDocs = eCon.Children().First();
            var eDoc = eDocs["document"];
            var fields = eDoc.ToObject<Dictionary<string, ExtractedContentField>>();
            return fields;
        }

        public decimal? confidence { get; set; }
        public double? top { get; set; }

        public double? left { get; set; }

        public double? width { get; set; }

        public double? height { get; set; }

        public int? page { get; set; }

        public string value { get; set; }
    }
}