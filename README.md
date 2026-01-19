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

## Uso en aplicaciones Razor / Blazor

La librería puede utilizarse directamente en aplicaciones **Blazor Web App** o **Blazor Server**, usando `IBrowserFile` como fuente de la imagen.

---

### `_Imports.razor`

Agregar los siguientes `@using` para facilitar el uso en componentes Razor:

```razor
@using MediaProcessorLibrary.Application
@using MediaProcessorLibrary.Domain
@using MediaProcessorLibrary.Application.UseCases
@using MediaProcessorLibrary.Application.ImageProcessing
@using MediaProcessorLibrary.Application.Results
```

---

### Inyección del servicio

En el componente Razor donde se procese la imagen:

```razor
@inject IImageProcessingService imageProcessingService
@inject IConfiguration Configuration
```

---

### Ejemplo completo de implementación

```csharp
private async Task UploadFile(IBrowserFile file)
{
    int maxFileSizeMB = 2 * 1024 * 1024;

    var fullOutputDirectory =
        Configuration["FileStorageSettings:PhysicalImagePath"];
   
    using var browserStream = file.OpenReadStream(maxFileSizeMB);
    using var memoryStream = new MemoryStream();

    await browserStream.CopyToAsync(memoryStream);
    memoryStream.Position = 0;

    var request = new ImageProcessingRequest
    {
        ImageStream = memoryStream,
        OutputDirectory = fullOutputDirectory,
        OutputFileName = "Example",
        MaxSizeBytes = maxFileSizeMB,
        Quality = 80,
        MaxWidth = 2000,
        MaxHeight = 2000
    };

    var result = await imageProcessingService.ImageProcessAsync(request);

    if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Value))
    {
        var fileName = Path.GetFileName(result.Value);

        var publicPath =
            Configuration["FileStorageSettings:PublicImagePath"];

        DefaultImgLarge = $"{publicPath}/{fileName}";
    }
    else
    {
        var message = result.Error switch
        {
            ErrorCode.ImageTooLarge   => "La imagen supera el tamaño permitido.",
            ErrorCode.InvalidImage    => "La imagen no es válida.",
            ErrorCode.CorruptedImage  => "La imagen está dañada.",
            ErrorCode.Unauthorized    => "No tienes permisos para guardar la imagen.",
            ErrorCode.IOError         => "Error de escritura en disco.",
            _                         => "Ocurrió un error inesperado."
        };       
    }

    await InvokeAsync(StateHasChanged);
}
```

---

## Notas importantes

* La librería **no depende de `wwwroot`**
* La ruta física y la ruta pública se definen vía `appsettings.json`
* En base de datos debe guardarse **la ruta pública**, no el path físico
* El consumidor decide cómo exponer las imágenes (UI, API, CDN, etc.)

---
