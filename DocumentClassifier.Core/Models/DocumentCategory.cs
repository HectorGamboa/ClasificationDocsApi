using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DocumentClassifier.Core.Models
{
    public class DocumentCategory
    {
        [JsonPropertyName("CategoryId")]
        public string CategoryId { get; set; }

        [JsonPropertyName("CategoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("Keywords")]
        public List<string> Keywords { get; set; }
    }
}
