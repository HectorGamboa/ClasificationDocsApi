# Sistema de Clasificación de Documentos

Este proyecto es una solución en .NET 8 diseñada para clasificar automáticamente documentos escaneados. Utiliza una combinación de reglas basadas en palabras clave, OCR y un modelo de Machine Learning para determinar a qué categoría pertenece cada documento y moverlo a la carpeta correspondiente.

## Arquitectura

La solución está organizada en una arquitectura limpia con los siguientes proyectos:

-   `DocumentClassifier.ConsoleApp`: La aplicación principal que orquesta el proceso de clasificación.
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
-   **Mejorar la Precisión:** Para obtener resultados precisos, necesitas proporcionar un conjunto de datos de entrenamiento mucho más grande y representativo de tus documentos. Añade más ejemplos al archivo CSV, asegurándote de que cada línea contenga una muestra de texto y la categoría correspondiente (ej. `mi primer titulo,FP-01`).

La aplicación entrenará y guardará automáticamente el modelo (`document-classifier-model.zip`) en la carpeta de salida la primera vez que se ejecute.

### 3. Procesamiento de PDFs

La implementación de Tesseract incluida está configurada para procesar archivos de **imagen**. Para procesar archivos PDF, necesitarás convertir cada página del PDF a una imagen y luego pasarla al servicio de OCR.

Puedes lograr esto utilizando una librería adicional como `PdfiumViewer`. Esta funcionalidad no está incluida, pero puede ser añadida modificando el `TesseractOcrService` para manejar archivos PDF.

## Ejecución de la Aplicación

1.  Coloca tus documentos escaneados en la carpeta `DocumentClassifier.ConsoleApp/bin/Debug/net8.0/Input`.
2.  Ejecuta la aplicación de consola.
3.  Los documentos clasificados se moverán a la carpeta `DocumentClassifier.ConsoleApp/bin/Debug/net8.0/Clasificados`, organizados por grupo y categoría.
