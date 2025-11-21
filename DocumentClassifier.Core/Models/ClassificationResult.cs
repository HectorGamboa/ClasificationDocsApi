namespace DocumentClassifier.Core.Models
{
    public class ClassificationResult
    {
        public bool Success { get; set; }
        public string GroupName { get; set; }
        public string CategoryId { get; set; }
        public string Message { get; set; }
    }
}
