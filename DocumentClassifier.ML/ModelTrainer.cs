using Microsoft.ML;
using DocumentClassifier.Core.Models; // Actualizado
using System;
using System.IO;

namespace DocumentClassifier.ML
{
    public class ModelTrainer
    {
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;
        private DataViewSchema _dataSchema;

        public ModelTrainer()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public void Train(string trainingDataPath)
        {
            // Cargar los datos
            var dataView = _mlContext.Data.LoadFromTextFile<DocumentData>(trainingDataPath, separatorChar: ',', hasHeader: false);
            _dataSchema = dataView.Schema;

            // Construir el pipeline de entrenamiento
            var pipeline = ProcessData();

            // Entrenar el modelo
            _trainedModel = pipeline.Fit(dataView);
        }

        private EstimatorChain<Microsoft.ML.Transforms.KeyToValueMappingTransformer> ProcessData()
        {
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Category", outputColumnName: "Label")
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Text", outputColumnName: "TextFeaturized"))
                .Append(_mlContext.Transforms.Concatenate("Features", "TextFeaturized"))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            return pipeline;
        }

        public DocumentPrediction Predict(DocumentData input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<DocumentData, DocumentPrediction>(_trainedModel);
            return predictionEngine.Predict(input);
        }

        public void SaveModel(string modelPath)
        {
            _mlContext.Model.Save(_trainedModel, _dataSchema, modelPath);
        }

        public void LoadModel(string modelPath)
        {
            _trainedModel = _mlContext.Model.Load(modelPath, out _dataSchema);
        }
    }
}
