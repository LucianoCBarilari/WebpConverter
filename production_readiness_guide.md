# Production Readiness & Remediation Guide

This document outlines the critical bugs, architectural improvements, and configuration changes required to safely deploy the **MediaProcessing** project to a production environment.

---

## 1. Critical Bugs & Runtime Exceptions

The following issues will cause immediate failures or crashes in production and must be resolved first:

### 1.1. Missing Dependency Injection (DI) Registration
*   **Location:** [Program.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Program.cs#L39-L43)
*   **Description:** The [ImageProcessingService](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingService.cs) constructor depends on `FileService`, `DirectoryService`, and `WebPCompressor`. None of these three classes are registered in the DI container in `Program.cs`.
*   **Impact:** Any request to the image processing endpoints will crash in runtime with an `InvalidOperationException` stating that `IImageProcessingService` cannot be resolved.
*   **Remediation:** Register these classes in the DI container:
    ```csharp
    builder.Services.AddScoped<FileService>();
    builder.Services.AddScoped<DirectoryService>();
    builder.Services.AddScoped<WebPCompressor>();
    ```

### 1.2. Broken Image Dimension Validation (Always Fails)
*   **Location:** [ImageProcessingAppService.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingAppService.cs#L84-L85) & [ImageValidator.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/Tools/ImageValidator.cs#L52-L53)
*   **Description:** `ImageProcessingAppService` instantiates the processing request setting `MaxWidth = 0` and `MaxHeight = 0`. The validator subsequently checks:
    ```csharp
    if (info.Width > maxWidth || info.Height > maxHeight)
        return ResultMedia.Fail(ErrorCode.InvalidDimensions);
    ```
*   **Impact:** Since any valid image has dimensions larger than 0x0, **every single image upload will be rejected** with `ErrorCode.InvalidDimensions`.
*   **Remediation:** Remove the width and height checks from the validator completely, or ensure valid default boundaries are supplied.

### 1.3. Unreachable File Deletion Code (Dead Code)
*   **Location:** [ImageProcessingAppService.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingAppService.cs#L31) & [L95-L98](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingAppService.cs#L95-L98)
*   **Description:** The variable `newImage` is declared as `private readonly string? newImage = string.Empty;` and is never updated or reassigned. 
*   **Impact:** The conditional block that invokes `_deleteimgHandler.Delete(...)` will never execute. Any replacement of files will fail to clean up the older files.
*   **Remediation:** If the service is intended to support replacing or updating an image, pass the name of the file to be replaced (`existingImageName`) as a parameter to `ProcessImage`.

### 1.4. Ineffective Call to `DirectoryService.FolderExist`
*   **Location:** [ImageProcessingService.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingService.cs#L55)
*   **Description:** The return value of `directoryService.FolderExist(...)` is discarded. Furthermore, [DirectoryService.FolderExist](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Infrastructure/FileSystem/DirectoryService.cs#L80-L86) checks folders relative to `Directory.GetCurrentDirectory()` unless an absolute path is provided.
*   **Impact:** This call does not validate folder existence in a meaningful way or create the directory. (Note: `FileService.SaveAsync` handles directory creation internally, making this call redundant).
*   **Remediation:** Remove this redundant call or use it properly to return a failure `ResultMedia` if the output directory is invalid.

---

## 2. Infrastructure & Configuration Issues

### 2.1. Incomplete Serilog Setup
*   **Location:** [Program.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Program.cs#L16-L19)
*   **Description:** Serilog is configured as a bootstrap logger, but is never hooked up as the logging provider for the host builder.
*   **Impact:** ASP.NET Core will fall back to using default console loggers instead of utilizing Serilog sinks (File, Console, etc.) and configuration enrichers.
*   **Remediation:** Call `builder.Host.UseSerilog()` during program initialization:
    ```csharp
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());
    ```

### 2.2. Missing Global Exception Handling Middleware
*   **Location:** [Program.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Program.cs#L92)
*   **Description:** While `app.UseExceptionHandler();` is called, no custom exception handler or Developer Page fallback is configured for production.
*   **Impact:** Unhandled exceptions (like network dropouts or unhandled file locks) will leak server stack traces to the API consumers or crash silently without proper tracking.
*   **Remediation:** Register a custom exception handler middleware or configure `UseExceptionHandler` with a standard fallback endpoint returning `ProblemDetails`.

---

## 3. Architecture & Code Quality Recommendations

### 3.1. Overlapping Service Layers (Violates DRY)
*   **Location:** [ImageProcessingAppService.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingAppService.cs) & [ImageProcessingService.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessingService.cs)
*   **Description:** Both classes duplicate file/size validations and contain overlapping logic. `ImageProcessingAppService` acts as an adapter, copying the file stream to a memory stream, while `ImageProcessingService` acts as the domain processor.
*   **Remediation:** Consolidate these layers or establish strict domain boundaries. E.g., validation rules should reside in a single place.

### 3.2. Hardcoded Values in Controller
*   **Location:** [ImageProcessorController.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Feature/ProcessImage/ImageProcessorController.cs#L40-L46)
*   **Description:** Constants like `maxFileSize` (2MB) and `quality` (80) are hardcoded inline inside the controller's action method.
*   **Remediation:** Move these parameters to `appsettings.json` and inject them using `IOptions<ImageProcessingSettings>`.

### 3.3. Restrictive Filename Sanitization
*   **Location:** [Utils.cs](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/Common/Utils.cs#L12-L15)
*   **Description:** The helper method `GenerateFileName` rejects any filename containing special characters (including spaces, dashes, or dots) by returning `string.Empty`, which causes the service flow to fail with `ErrorCode.FileNameEmpty`.
*   **Remediation:** Change the behavior to sanitise/replace invalid characters (e.g. replacing spaces with underscores) instead of failing the execution.

### 3.4. ImageSharp (SixLabors) Licensing Audit
*   **Location:** [MediaProcessing.csproj](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/MediaProcessing.csproj#L15)
*   **Description:** The project uses `SixLabors.ImageSharp` version `3.1.12`.
*   **Warning:** 
    > [!IMPORTANT]
    > Since version 3.x, SixLabors has transitioned to a split license (License Peer-to-Peer / Commercial). An audit must be performed to ensure compliance before deploying this software in commercial production environments.
