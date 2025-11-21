using System;
using System.IO;
using System.Threading.Tasks;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Services;
using DocumentClassifier.Infrastructure.Services;
using DocumentClassifier.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DocumentClassifier.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;

            // Rutas de carpetas y archivos
            var basePath = AppContext.BaseDirectory;
            var inputPath = Path.Combine(basePath, "Input");
            var classifiedPath = Path.Combine(basePath, "Clasificados");
            var categoriesJsonPath = Path.Combine(basePath, "categories.json");
            var modelPath = Path.Combine(basePath, "document-classifier-model.zip");
            var trainingDataPath = Path.Combine(basePath, "..", "DocumentClassifier.ML", "Data", "sample-data.csv");

            // Crear carpetas necesarias
            Directory.CreateDirectory(inputPath);
            Directory.CreateDirectory(classifiedPath);

            // Entrenar el modelo si no existe
            if (!File.Exists(modelPath))
            {
                Console.WriteLine("Entrenando el modelo de ML...");
                var trainer = new ModelTrainer();
                trainer.Train(trainingDataPath);
                trainer.SaveModel(modelPath);
                Console.WriteLine("Modelo entrenado y guardado.");
            }

            // Crear un archivo de ejemplo para procesar
            File.WriteAllText(Path.Combine(inputPath, "ejemplo-rfc.txt"), "Este es un documento con un RFC y CURP");

            var classifier = serviceProvider.GetRequiredService<IDocumentClassifierService>();

            Console.WriteLine("Iniciando clasificación de documentos...");

            var files = Directory.GetFiles(inputPath);
            foreach (var file in files)
            {
                Console.WriteLine($"Procesando archivo: {Path.GetFileName(file)}");

                var text = await classifier.ExtractText(file);
                var result = classifier.ApplyRules(text);

                if (!result.Success)
                {
                    Console.WriteLine("No se aplicaron reglas, usando modelo de ML...");
                    result = await classifier.PredictCategory(text);
                }

                if (result.Success)
                {
                    Console.WriteLine($"Clasificado como: {result.GroupName}/{result.CategoryId} - {result.Message}");
                    classifier.MoveToFolder(file, result);
                }
                else
                {
                    Console.WriteLine("No se pudo clasificar el documento.");
                }
            }

            Console.WriteLine("Clasificación completada.");
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IOcrService, TesseractOcrService>();
                    services.AddSingleton<IDocumentClassifierService>(provider =>
                        new DocumentClassifierService(
                            provider.GetRequiredService<IOcrService>(),
                            Path.Combine(AppContext.BaseDirectory, "categories.json"),
                            Path.Combine(AppContext.BaseDirectory, "Clasificados"),
                            Path.Combine(AppContext.BaseDirectory, "document-classifier-model.zip")
                        ));
                });
    }
}
