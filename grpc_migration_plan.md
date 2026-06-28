# Pure gRPC Service Migration & Architecture Plan

This document outlines the final architecture and remediation plan to convert the project into a **pure gRPC microservice** (removing all HTTP/REST/Controller layers). The service acts as a dedicated, high-performance image processing agent to validate, convert to WebP, compress, and save images.

---

## 1. Pure gRPC Architecture Overview

In a pure gRPC service, we eliminate all MVC, Web API Controllers, Swashbuckle/Swagger, and HTTP/REST middleware. The service handles requests exclusively over HTTP/2 using the gRPC protocol.

```
[gRPC Client]
      │ (HTTP/2 / Protobuf)
      ▼
┌────────────────────────────────────────────────────────┐
│ MediaProcessorGrpcService                              │ (gRPC Endpoint Adapter)
└──────────┬─────────────────────────────────────────────┘
           │ (Invokes Handler Command)
           ▼
┌────────────────────────────────────────────────────────┐
│ ProcessImageHandler (VSA Core Logic)                   │ (Image Validation & Saving flow)
└──────────┬───────────────────┬─────────────────────────┘
           │                   │
           ▼                   ▼
┌────────────────────┐   ┌──────────────────────────────┐
│ WebPCompressor     │   │ FileSystem Services          │ (Infrastructure Tasks)
│ (ImageSharp v3)    │   │ (Save/Delete files)          │
└────────────────────┘   └──────────────────────────────┘
```

By keeping the Vertical Slice Architecture (VSA) handler pattern, the core business rules are preserved, but the entry boundary is changed from a Controller to a gRPC Service Class.

---

## 2. Updated Project Dependencies

To strip away ASP.NET Web API overhead and configure a pure gRPC service, update [MediaProcessing.csproj](file:///C:/Users/Developer/Documents/Develop/CustomerProjects/MediaProcessing/MediaProcessing/MediaProcessing.csproj) as follows:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core gRPC dependency -->
    <PackageReference Include="Grpc.AspNetCore" Version="2.63.0" />
    <PackageReference Include="SixLabors.ImageSharp" Version="3.1.12" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- Auto-compiles Protobuf contracts into C# Server Stub -->
    <Protobuf Include="Protos\media_processor.proto" GrpcServices="Server" />
  </ItemGroup>

</Project>
```

---

## 3. Pure gRPC Folder Structure

All REST files, request DTOs linked to HTTP forms, and OpenAPI tools are completely removed:

```
MediaProcessing/
│
├── Common/                      # Shared domain objects
│   ├── Enums/
│   │   └── ErrorCode.cs
│   └── Results/
│       ├── ResultMedia.cs
│       ├── MediaErrors.cs
│       └── Operation.cs
│
├── Feature/                     # Vertically sliced feature logic
│   ├── DeleteImage/
│   │   └── DeleteImageHandler.cs
│   │
│   └── ProcessImage/
│       ├── ProcessImageHandler.cs (Executes validation, calls WebP compressor)
│       ├── ImageProcessingRequest.cs (The command payload)
│       └── ImageValidator.cs (Shared or slice-bound validation utility)
│
├── Infrastructure/              # Under-the-hood engine implementations
│   ├── Compression/
│   │   └── WebPCompressor.cs
│   └── FileSystem/
│       ├── FileService.cs
│       └── DirectoryService.cs
│
├── Protos/                      # Proto service contract
│   └── media_processor.proto
│
├── Services/                    # gRPC boundary implementations
│   └── MediaProcessorGrpcService.cs
│
├── appsettings.json
└── Program.cs                   # Pure gRPC server bootstrapping
```

---

## 4. C# & Protobuf Service Specifications

### 4.1. Protobuf Definition (`Protos/media_processor.proto`)
The client streams the image bytes in packets (`ProcessImageChunkRequest`) to avoid memory saturation, and the server returns the path to the saved WebP image.

```protobuf
syntax = "proto3";

package mediaprocessing;

option csharp_namespace = "MediaProcessing.Services";

service MediaProcessor {
  // Client streams chunks of the image to convert, compress and save
  rpc ProcessImage (stream ProcessImageChunkRequest) returns (ProcessImageResponse);
  
  // Deletes an image from storage
  rpc DeleteImage (DeleteImageRequest) returns (DeleteImageResponse);
}

message ProcessImageChunkRequest {
  oneof payload {
    ImageMetadata metadata = 1;
    bytes chunk_data = 2;
  }
}

message ImageMetadata {
  string file_name = 1;
  int64 max_file_size = 2;
  int32 quality = 3;
}

message ProcessImageResponse {
  bool is_success = 1;
  string public_path = 2;
  string error_message = 3;
  int32 error_code = 4;
}

message DeleteImageRequest {
  string folder_path = 1;
  string file_name = 2;
}

message DeleteImageResponse {
  bool is_success = 1;
  string error_message = 2;
  int32 error_code = 4;
}
```

### 5. Pure gRPC Bootstrap Setup (`Program.cs`)
All endpoints mapping controllers, Swagger configurations, and anti-forgery systems are removed, leaving a clean gRPC host.

```csharp
using MediaProcessing.Feature.DeleteImage;
using MediaProcessing.Feature.ProcessImage;
using MediaProcessing.Infrastructure.Compression;
using MediaProcessing.Infrastructure.FileSystem;
using MediaProcessing.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// Wire Serilog as the main logging provider
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Register core features (VSA Handlers)
builder.Services.AddScoped<ProcessImageHandler>();
builder.Services.AddScoped<DeleteImageHandler>();

// Register Infrastructure services (properly resolved for DI)
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<WebPCompressor>();

// Enable gRPC Services
builder.Services.AddGrpc();

var app = builder.Build();

// Direct gRPC Pipeline mapping
app.MapGrpcService<MediaProcessorGrpcService>();

app.Run();
```
