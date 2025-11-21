using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocumentClassifier.Core.Interfaces;
using DocumentClassifier.Core.Services;
using DocumentClassifier.Infrastructure.Services;
using System.IO;
using System;

namespace DocumentClassifier.Tests
{
    [TestClass]
    public class DocumentClassifierServiceTests
    {
        private IDocumentClassifierService _classifier = null!;

        [TestInitialize]
        public void Setup()
        {
            // Configurar las rutas y servicios necesarios para las pruebas
            var basePath = AppContext.BaseDirectory;
            var solutionRoot = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", ".."));
            var categoriesJsonPath = Path.Combine(solutionRoot, "DocumentClassifier.ConsoleApp", "categories.json");

            // Usar el servicio de OCR simulado para las pruebas
            var ocrService = new TesseractOcrService();

            // Usar un modelo dummy ya que no probamos la lógica de ML aquí
            _classifier = new DocumentClassifierService(ocrService, categoriesJsonPath, "dummy_path", "dummy_model.zip");
        }

        [TestMethod]
        public void ApplyRules_Should_Classify_RFC_Correctly()
        {
            // Arrange
            var text = "documento con registro federal de contribuyentes";

            // Act
            var result = _classifier.ApplyRules(text);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("DP", result.GroupName);
            Assert.AreEqual("04", result.CategoryId);
        }

        [TestMethod]
        public void ApplyRules_Should_Classify_CURP_Correctly()
        {
            // Arrange
            var text = "mi curp es ABC...";

            // Act
            var result = _classifier.ApplyRules(text);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("DP", result.GroupName);
            Assert.AreEqual("05", result.CategoryId);
        }

        [TestMethod]
        public void ApplyRules_Should_Classify_Professional_Title_Correctly()
        {
            // Arrange
            var text = "titulo profesional de ingeniero";

            // Act
            var result = _classifier.ApplyRules(text);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("FP", result.GroupName);
            Assert.AreEqual("01", result.CategoryId);
        }

        [TestMethod]
        public void ApplyRules_Should_Fail_For_Unrecognized_Text()
        {
            // Arrange
            var text = "este es un documento sin palabras clave";

            // Act
            var result = _classifier.ApplyRules(text);

            // Assert
            Assert.IsFalse(result.Success);
        }
    }
}
