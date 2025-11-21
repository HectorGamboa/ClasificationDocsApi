using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DocumentClassifier.Core.Models
{
    public class DocumentGroup
    {
        [JsonPropertyName("GroupName")]
        public string GroupName { get; set; }

        [JsonPropertyName("GroupDescription")]
        public string GroupDescription { get; set; }

        [JsonPropertyName("Categories")]
        public List<DocumentCategory> Categories { get; set; }
    }
}
