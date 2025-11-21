using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Services;
using DocumentClassifier.Infrastructure.Services;
using DocumentClassifier.ML;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Configuración de Inyección de Dependencias ---

// Rutas de carpetas y archivos
var basePath = AppContext.BaseDirectory;
var classifiedPath = Path.Combine(basePath, "Clasificados");
var categoriesJsonPath = Path.Combine(basePath, "categories.json");
var modelPath = Path.Combine(basePath, "document-classifier-model.zip");
var trainingDataPath = Path.Combine(basePath, "..", "DocumentClassifier.ML", "Data", "sample-data.csv");


// Crear carpetas necesarias
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

// Registrar los servicios
builder.Services.AddSingleton<IOcrService, TesseractOcrService>();
builder.Services.AddSingleton<IDocumentClassifierService>(provider =>
    new DocumentClassifierService(
        provider.GetRequiredService<IOcrService>(),
        categoriesJsonPath,
        classifiedPath,
        modelPath
    ));

// ----------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
