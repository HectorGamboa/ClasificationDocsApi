using Microsoft.ML.Data;

namespace DocumentClassifier.Core.Models
{
    public class DocumentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }
    }
}
