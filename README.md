# Sistema de Clasificación de Documentos (Web API)

Este proyecto es una solución en .NET 8 diseñada para clasificar automáticamente documentos escaneados. Utiliza una combinación de reglas basadas en palabras clave, OCR y un modelo de Machine Learning para determinar a qué categoría pertenece cada documento.

## Arquitectura

La solución está organizada en una arquitectura limpia con los siguientes proyectos:

-   `DocumentClassifier.WebApi`: Una API de ASP.NET Core que expone un endpoint para la clasificación de documentos.
-   `DocumentClassifier.Core`: Contiene la lógica de negocio principal, incluyendo los servicios, interfaces y modelos de dominio.
-   `DocumentClassifier.Infrastructure`: Se encarga de las implementaciones externas, como el servicio de OCR.
-   `DocumentClassifier.ML`: Gestiona el entrenamiento y la predicción del modelo de Machine Learning.
-   `DocumentClassifier.Tests`: Contiene las pruebas unitarias.

## Cómo Empezar

### 1. Configurar el Entorno

La solución está lista para ejecutarse en modo de demostración, pero para procesar documentos reales, necesitarás configurar el servicio de OCR.

**Configuración de Tesseract OCR:**

El servicio de OCR utiliza Tesseract. Por defecto, la implementación está **simulada** para permitir que la aplicación se ejecute sin dependencias externas. Para habilitar el OCR real, sigue estos pasos:

1.  **Instala el motor de Tesseract:** Descarga e instala el motor de Tesseract para tu sistema operativo desde la [página oficial de Tesseract](https://tesseract-ocr.github.io/tessdoc/Downloads.html).
2.  **Descarga los datos de idioma:** Descarga el archivo de datos entrenados para español (`spa.traineddata`) desde el [repositorio de datos de Tesseract](https://github.com/tesseract-ocr/tessdata) y colócalo en un directorio llamado `tessdata` dentro de tu proyecto `DocumentClassifier.Infrastructure`.
3.  **Habilita la implementación real:**
    *   Abre el archivo `DocumentClassifier.Infrastructure/Services/TesseractOcrService.cs`.
    *   Comenta o elimina el método `ExtractTextAsync` de la implementación simulada.
    *   Descomenta el bloque de código de la "Implementación Real con Tesseract".
    *   Asegúrate de que la ruta a `tessdata` sea correcta en el constructor. Si es necesario, proporciona la ruta al ejecutable de Tesseract.

### 2. Entrenamiento del Modelo de Machine Learning

El modelo de ML está diseñado para clasificar documentos cuando las reglas de palabras clave no son suficientes.

-   **Datos de Entrenamiento:** Se incluye un pequeño archivo de ejemplo (`sample-data.csv`) en el proyecto `DocumentClassifier.ML/Data`. Este archivo es solo para fines de demostración.
-   **Mejorar la Precisión:** Para obtener resultados precisos, necesitas proporcionar un conjunto de datos de entrenamiento mucho más grande y representativo de tus documentos.

La API entrenará y guardará automáticamente el modelo (`document-classifier-model.zip`) en la carpeta de salida la primera vez que se inicie.

### 3. Procesamiento de PDFs

La implementación de Tesseract incluida está configurada para procesar archivos de **imagen**. Para procesar archivos PDF, necesitarás convertir cada página del PDF a una imagen y luego pasarla al servicio de OCR. Puedes lograr esto utilizando una librería adicional como `PdfiumViewer`.

## Ejecución de la API

1.  Inicia el proyecto `DocumentClassifier.WebApi`.
2.  La API estará disponible en `https://localhost:<puerto>`.
3.  Puedes usar una herramienta como `curl` o Postman para enviar documentos al endpoint de clasificación.

### Ejemplo de Uso con `curl`

```bash
curl -X POST -F "file=@/ruta/a/tu/documento.png" https://localhost:<puerto>/api/classification/upload
```

La respuesta será un JSON con el resultado de la clasificación. Los archivos clasificados se guardarán en la carpeta `DocumentClassifier.WebApi/bin/Debug/net8.0/Clasificados`.
