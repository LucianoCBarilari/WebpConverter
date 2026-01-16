# MediaProcessorLibrary

Librería .NET 8 para **procesamiento de imágenes** basada en `Stream`, diseñada para ser **simple de integrar**, **predecible** y **eficiente**.

Permite validar, convertir y guardar imágenes en formato **WebP**, evitando el uso de excepciones como control de flujo mediante **Result Pattern**.

---

## Qué hace

- Valida imágenes (tamaño, dimensiones, integridad)
- Convierte imágenes a **WebP**
- Comprime según calidad configurada
- Guarda el archivo en una carpeta destino
- Devuelve resultados claros (`Result<T>`) en lugar de lanzar excepciones

---

## Requisitos

- .NET 8
- Cualquier tipo de aplicación (.NET Console, Web API, Worker, etc.)

---

## Instalación

Agregar referencia al proyecto o paquete:

```bash
dotnet add reference MediaProcessorLibrary
````

---

## Configuración (una sola línea)

```csharp
builder.Services.AddMediaProcessor();
```

Esto registra automáticamente todas las dependencias necesarias.

---

## Uso básico

```csharp
var request = new ImageProcessingRequest
{
    ImageStream = stream,
    OutputDirectory = "output",
    OutputFileName = "image",
    MaxWidth = 2000,
    MaxHeight = 2000,
    MaxSizeBytes = 2 * 1024 * 1024,
    Quality = 80
};

var result = await imageProcessingService.ImageProcessAsync(request);

if (!result.IsSuccess)
{
    // manejar error con result.Error
    return;
}

string savedPath = result.Value;
```

---

## Manejo de errores

La librería **no lanza excepciones** para casos esperables.

Todos los resultados devuelven:

* `IsSuccess`
* `Value` (si aplica)
* `Error` (`ErrorCode`)
* `Operation`

Ejemplos de errores:

* `InvalidStream`
* `ImageTooLarge`
* `CorruptedImage`
* `Unauthorized`
* `IOError`

---
