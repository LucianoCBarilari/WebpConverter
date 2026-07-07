# WebpConverter

Self-hosted **gRPC Service** to convert and compress images to **WebP**, powered by SkiaSharp and .NET 10.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/) [![License](https://img.shields.io/badge/license-Apache%202.0-blue)](LICENSE) [![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker)](docker-compose.yml)

## Overview

WebpConverter is a gRPC service designed to receive image payloads, convert them to **WebP** format, compress them, and store them on your server. It returns the public URL path of the saved file, ready to be served by a web server like Nginx or Caddy.

## Features

- **gRPC API**: High-performance, binary communication using Protobuf.
- **WebP & GIF**: Converts raster images to WebP and passes GIFs through to preserve animations.
- **Subfolder Routing**: Route images into specific CDN subdirectories per request using the `subfolder` parameter.
- **Validation & Cleanup**: Validates file integrity and optionally deletes old images when replacing them.
- **Flexible Setup**: Run via Docker or build from source using the .NET SDK.

## Quick Start

You can run this project using Docker (recommended) or by building it locally with the .NET SDK.

### Option A: Run via Docker (Recommended)

1. Clone the repository.
2. In the root of the project, create an `.env` file based on the example:
   ```bash
   cp .env.example .env
   ```
3. Start the container:
   ```bash
   docker compose up -d
   ```
   The gRPC server will be available at `localhost:8080`.

### Option B: Build and Run Locally

1. Clone the repository and navigate to the `WebpConverter` folder.
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run the project:
   ```bash
   dotnet run
   ```
   The service will start on the port specified in your `Properties/launchSettings.json` or `appsettings.json`.

## Configuration

You can configure the application using either **Environment Variables** (`.env` when using Docker) or directly in `appsettings.json` when running locally. Both methods configure the same settings:

| Setting | Docker / `.env` Variable | Description |
|---------|-------------------------|-------------|
| Port | `HOST_PORT` | External port exposed (default: `8080`) |
| Storage Path | `HOST_CDN_PATH` | Directory where images are saved |
| Compression | `COMPRESSION_QUALITY` | WebP compression quality (0–100) |
| Max Size | `MAX_FILE_SIZE` | Maximum upload size in bytes (default: 5 MB) |

*If you are running the project locally without Docker, simply update the `AppOptions` section in your `appsettings.json` file to match these values.*

### Example `appsettings.json`

```json
{
  "AppOptions": {
    "StoragePath": "/app/cdn-images",
    "PublicUrlPath": "/images",
    "ImageCompressionQuality": 80,
    "MaxFileSizeBytes": 5242880
  },
  "LogOptions": {
    "FolderPath": "Logs",
    "FileName": "log.txt",
    "MinimumLevel": "Information"
  }
}
```

## Usage

This service communicates via **gRPC**. To interact with it, you must use the `image_processor.proto` contract.

### The Protobuf Contract (`image_processor.proto`)

```proto
syntax = "proto3";
option csharp_namespace = "WebpConverter";

service ImageProcessor {
  rpc ConvertImage (ConvertImageRequest) returns (ConvertImageReply);
}

message ConvertImageRequest {
  bytes image_data     = 1;  // Raw image bytes (or Base64 string if testing via Postman)
  string file_name     = 2;  // Base name for the output file (no extension)
  string previous_file_name = 3; // Optional: Name of a previously saved file to delete
  string subfolder     = 4;  // Optional: CDN subdirectory for this caller (e.g. 'ecommerce', 'blog')
}

message ConvertImageReply {
  bool   success       = 1;
  string public_path   = 2;  // The public URL path of the saved image
  string error_code    = 3;  // Empty if success, otherwise contains the error type
  string error_message = 4;
}
```

### Integration & Testing

Generate your client using the `.proto` file in your preferred language (C#, Node.js, Python, etc.), and pass the image bytes directly to `image_data`.

> **Testing via Postman:** If you are testing this gRPC endpoint using tools like Postman, you must manually convert your test image to a **Base64 string** and place it in the `image_data` field of the JSON payload.

If successful, `success` will be `true`, and `public_path` will contain the URL:

```json
{
  "success": true,
  "public_path": "/images/ecommerce/banner-abc123.webp",
  "error_code": "",
  "error_message": ""
}
```

## License

This project is licensed under the Apache License 2.0 — see the [LICENSE](LICENSE) file for details.
