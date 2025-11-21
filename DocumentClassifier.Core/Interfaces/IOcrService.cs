using System.Threading.Tasks;

namespace DocumentClassifier.Core.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractTextAsync(string filePath);
    }
}
