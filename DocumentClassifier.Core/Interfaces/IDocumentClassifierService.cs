using DocumentClassifier.Core.Models;
using System.Threading.Tasks;

namespace DocumentClassifier.Core.Interfaces
{
    public interface IDocumentClassifierService
    {
        Task<string> ExtractText(string filePath);
        ClassificationResult ApplyRules(string text);
        Task<ClassificationResult> PredictCategory(string text);
        void MoveToFolder(string sourcePath, ClassificationResult result);
    }
}
