using Microsoft.AspNetCore.Mvc;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Models;

namespace DocumentClassifier.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassificationController : ControllerBase
    {
        private readonly IDocumentClassifierService _classifier;
        private readonly ILogger<ClassificationController> _logger;

        public ClassificationController(IDocumentClassifierService classifier, ILogger<ClassificationController> logger)
        {
            _classifier = classifier;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAndClassify(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha subido ningún archivo.");
            }

            var tempPath = Path.GetTempFileName();
            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"Procesando archivo: {file.FileName}");

                var text = await _classifier.ExtractText(tempPath);
                var result = _classifier.ApplyRules(text);

                if (!result.Success)
                {
                    _logger.LogInformation("No se aplicaron reglas, usando modelo de ML...");
                    result = await _classifier.PredictCategory(text);
                }

                if (result.Success)
                {
                    _logger.LogInformation($"Clasificado como: {result.GroupName}/{result.CategoryId} - {result.Message}");
                    // En un entorno de API, mover el archivo original podría no ser lo deseado.
                    // En su lugar, guardaremos una copia en la carpeta clasificada.
                    // La lógica original 'MoveToFolder' se adaptará para este propósito.

                    // Simulación del movimiento por ahora, ya que 'MoveToFolder' elimina el original
                    _classifier.MoveToFolder(tempPath, result);

                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("No se pudo clasificar el documento.");
                    return NotFound(new ClassificationResult { Success = false, Message = "No se pudo clasificar el documento." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante la clasificación.");
                return StatusCode(500, "Ocurrió un error interno en el servidor.");
            }
            finally
            {
                // Asegurarse de que el archivo temporal se elimine si aún existe,
                // excepto si fue movido por MoveToFolder.
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
        }
    }
}
