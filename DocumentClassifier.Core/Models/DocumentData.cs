using Microsoft.ML.Data;

namespace DocumentClassifier.Core.Models
{
    public class DocumentData
    {
        [LoadColumn(0)]
        public string Text { get; set; }

        [LoadColumn(1)]
        public string Category { get; set; }
    }
}
