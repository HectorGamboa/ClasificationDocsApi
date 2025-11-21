using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;
// using DocumentClassifier.ML; // Ya no es necesario
// using DocumentClassifier.ML.Models; // Ya no es necesario
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ML;

namespace DocumentClassifier.Core.Services
{
    public class DocumentClassifierService : IDocumentClassifierService
    {
        private readonly IOcrService _ocrService;
        private readonly List<DocumentGroup> _documentGroups;
        private readonly string _classifiedFolderPath;
        // private readonly ModelTrainer _modelTrainer; // Esto necesita ser reimplementado
        private readonly PredictionEngine<DocumentData, DocumentPrediction> _predictionEngine;


        public DocumentClassifierService(IOcrService ocrService, string categoriesJsonPath, string classifiedFolderPath, string modelPath)
        {
            _ocrService = ocrService;
            var jsonContent = File.ReadAllText(categoriesJsonPath);
            _documentGroups = JsonSerializer.Deserialize<List<DocumentGroup>>(jsonContent);
            _classifiedFolderPath = classifiedFolderPath;

            if (File.Exists(modelPath))
            {
                var mlContext = new MLContext();
                var trainedModel = mlContext.Model.Load(modelPath, out _);
                _predictionEngine = mlContext.Model.CreatePredictionEngine<DocumentData, DocumentPrediction>(trainedModel);
            }
        }

        public async Task<string> ExtractText(string filePath)
        {
            return await _ocrService.ExtractTextAsync(filePath);
        }

        public ClassificationResult ApplyRules(string text)
        {
            var lowerText = text.ToLowerInvariant();
            foreach (var group in _documentGroups)
            {
                foreach (var category in group.Categories)
                {
                    foreach (var keyword in category.Keywords)
                    {
                        if (lowerText.Contains(keyword))
                        {
                            return new ClassificationResult
                            {
                                Success = true,
                                GroupName = group.GroupName,
                                CategoryId = category.CategoryId,
                                Message = $"Clasificado por palabra clave: {keyword}"
                            };
                        }
                    }
                }
            }

            return new ClassificationResult { Success = false, Message = "No se encontraron palabras clave." };
        }

        public Task<ClassificationResult> PredictCategory(string text)
        {
            if (_predictionEngine == null)
            {
                return Task.FromResult(new ClassificationResult { Success = false, Message = "El modelo de ML no está cargado." });
            }

            var prediction = _predictionEngine.Predict(new DocumentData { Text = text });
            var categoryParts = prediction.Category.Split('-');

            if (categoryParts.Length == 2)
            {
                return Task.FromResult(new ClassificationResult
                {
                    Success = true,
                    GroupName = categoryParts[0],
                    CategoryId = categoryParts[1],
                    Message = "Clasificado por modelo de ML."
                });
            }

            return Task.FromResult(new ClassificationResult { Success = false, Message = "El modelo de ML no pudo determinar la categoría." });
        }

        public void MoveToFolder(string sourcePath, ClassificationResult result)
        {
            if (!result.Success) return;

            var targetDirectory = Path.Combine(_classifiedFolderPath, result.GroupName, result.CategoryId);
            Directory.CreateDirectory(targetDirectory);

            var fileName = Path.GetFileName(sourcePath);
            var destinationPath = Path.Combine(targetDirectory, fileName);
            File.Move(sourcePath, destinationPath);
        }
    }
}
