# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.6.0] - 2026-08-28

### Added
- Added gRPC Health Checks (Grpc.AspNetCore.HealthChecks) for container orchestrator readiness/liveness probes.
- Propagated CancellationToken support throughout the pipeline (gRPC to SkiaSharp/IFileService) to cleanly abort processing on client disconnects.

### Changed
- Switched Docker base image from Azure Linux Distroless to Alpine Linux (spnet:10.0-alpine) to retain a small footprint while providing a shell (/bin/sh) for production debugging.
- Refactored FileService.CreateFile to explicitly call .Dispose() instead of using a discard variable.

### Security
- Fixed a path traversal vulnerability in ProcessImageHandler by sanitizing and validating that the target path strictly resolves within the configured base StoragePath.
