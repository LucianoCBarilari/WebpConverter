# WebpConverter

Self-hosted REST API to convert and compress images to **WebP**, powered by SkiaSharp and .NET 10.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/) [![License](https://img.shields.io/badge/license-Apache%202.0-blue)](LICENSE) [![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker)](docker-compose.yml)

## Overview

WebpConverter is a minimal HTTP API designed to receive image uploads, convert them to **WebP** format, compress them, and store them on your server. It returns the public URL path of the saved file, ready to be served by a web server like Nginx or Caddy.

## Features

- **WebP Conversion**: Converts raster images to WebP using [SkiaSharp](https://github.com/mono/SkiaSharp).
- **Configurable Compression**: Quality level adjustable from 0 to 100.
- **File Validation**: Validates file size and image integrity before processing.
- **Previous File Cleanup**: Optionally deletes an existing image when replacing it.
- **Structured Logging**: File and console logging via [Serilog](https://serilog.net/).
- **Docker Ready**: Single `docker compose up` to get running.

## Supported Input Formats

| Format | Supported |
|--------|-----------|
| JPEG / JPG | ✅ |
| PNG | ✅ |
| BMP | ✅ |
| GIF | ✅ *(first frame only — animation is not preserved)* |
| TIFF | ✅ |
| WebP | ✅ *(re-compressed)* |
| SVG | ❌ *(vector format, not supported)* |

## Quick Start

### 1. Configure environment

```bash
cp .env.example .env
```

Edit `.env` with your values:

```env
HOST_PORT=8080
HOST_CDN_PATH=/var/www/cdn/images
COMPRESSION_QUALITY=80
MAX_FILE_SIZE=5242880
```

### 2. Run

```bash
docker compose up -d
```

The API will be available at `http://your-host:8080`.

## Usage

### `POST /api/image-processor`

**Content-Type:** `multipart/form-data`

| Field | Required | Description |
|-------|----------|-------------|
| `File` | ✅ | The image file to convert |
| `FileName` | ❌ | Base name for the output file (no extension) |
| `PreviousFileName` | ❌ | Name of a previously saved WebP file to delete on success |

**Success — `200 OK`**

Returns the public URL path of the saved image:

```
"/images/my-photo-abc123.webp"
```

> The path prefix (`/images`) is configured via `AppOptions__PublicUrlPath`. This is the URL segment your web server (Nginx, Caddy, etc.) should map to the physical storage directory.

**Errors**

| Status | Title | Cause |
|--------|-------|-------|
| `400` | Validation Error | No file provided |
| `400` | Image Too Large | File exceeds `MAX_FILE_SIZE` |
| `400` | Invalid Image | File is not a recognized image format |
| `400` | Corrupted Image | File could not be decoded (e.g. SVG, corrupted data) |
| `403` | Forbidden | Write permission denied on the storage path |
| `500` | I/O Error | Disk write failure |

## Configuration

### Environment Variables (`.env`)

| Variable | Default | Description |
|----------|---------|-------------|
| `HOST_PORT` | `8080` | External port exposed on the host |
| `HOST_CDN_PATH` | `./local-cdn` | Host directory mounted as the image storage volume |
| `COMPRESSION_QUALITY` | `80` | WebP compression quality (0–100) |
| `MAX_FILE_SIZE` | `5242880` | Maximum upload size in bytes (default: 5 MB) |

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

> In Docker, `AppOptions` values are overridden by the environment variables defined in `docker-compose.yml`.

## Acknowledgments

- [SkiaSharp](https://github.com/mono/SkiaSharp) — Cross-platform image decoding and WebP encoding.
- [Serilog](https://serilog.net/) — Structured logging for .NET.
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — OpenAPI / Swagger UI.

## License

This project is licensed under the Apache License 2.0 — see the [LICENSE](LICENSE) file for details.
