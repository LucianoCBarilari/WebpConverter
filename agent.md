# Media Processing Agent Overview - Architectural Blueprint (agent.md)

## 🎯 Goal of this Document
This blueprint serves as an authoritative document for autonomous agents, detailing the precise folder structure, component responsibilities, and interaction points within the 'MediaProcessing' repository. It replaces abstract dependency descriptions with a concrete structural map.

## 🗺️ Project File and Folder Structure Overview
The codebase follows a standard layered architecture common in enterprise .NET solutions. The following hierarchy outlines relevant directories and components:

### 📂 Root Level (Project Scope)
*   `.gitignore`: Defines all files/folders that are ignored by Git (e.g., `Debug/`, `bin/`, `.vs/`, `node_modules/`). **Agents must respect these exclusions.**
*   `Dockerfile`: Crucial for build and deployment context. All deployment scripts must reference this file's logic.
*   `appsettings.*json`: Environment-specific configuration files (e.g., `appsettings.Development.json`). Agents require access to read these settings at runtime.

### 💾 Core Source Code (`src/`)
This directory holds the main compiled application code:
*   **`MediaProcessorLibrary/`:** **(The Core Business Logic)** Contains the primary `*.csproj` and business logic for media encoding, decoding, and transformation. This is the module agents interact with to perform actual work.
    *   *Expected Contents:* Service implementions, core algorithms, utility classes.

### 🔗 Service & Interface Boundaries (`services/`)
This layer defines how external components or internal modules communicate:
*   **`Services/`:** (gRPC Boundary Implementations) Contains service implementations (e.g., `MediaProcessorGrpcService.cs`). This is the primary API endpoint an agent will call to manage tasks or receive streamed data.

### 🛡️ Domain Models and Shared Contracts (`models/`) - *Recommended*
A dedicated place for type safety:
*   **Domain Objects:** Data Transfer Objects (DTOs), business entities, and shared types defining contract inputs and outputs across all modules.

## ✨ Interconnectivity and Dependencies Flow
Instead of listing "dependencies" generally, this section maps out how components rely on each other:

1.  **Outer World $\longrightarrow$ Services Layer:** External calls hit the gRPC endpoint defined in `services/`.
2.  **Services Layer $\longrightarrow$ Core Logic:** The service layer validates inputs and delegates processing tasks to the dedicated functionality within `MediaProcessorLibrary/`.
3.  **Cross-Cutting Dependency:** All layers rely heavily on **Domain Models** (`models/`) for type conformity and safe data exchange.

## ✅ Agent Action Checklist (Focus Areas)
To ensure successful task execution, an operational agent must follow this sequence:

1.  **Initialization:** Use the `Dockerfile` context to identify the required project files and execute necessary build steps (`dotnet restore`, `dotnet build`).
2.  **Execution:** Focus primary actions on **Service Orchestration** (calling gRPC) and utilizing core logic from `MediaProcessorLibrary/`.
3.  **Data Integrity:** Always validate input media files against defined schemas (Models); never assume file type conformity.

---
***Agent Blueprint Status: Ready for Implementation Review.***
# Media Processing Agent Overview (agent.md)

## 📜 Purpose
This document provides a comprehensive analysis of the 'MediaProcessing' repository structure, architecture, dependencies, and core components. It serves as a guide for creating specialized AI agents that interact with and manage media processing tasks within this codebase.

## 📂 Project Structure & High-Level Components
The project appears to be organized according to standard modern .NET architectural patterns, suggesting separation of concerns between API/Services, Core Logic/Models, and the Build Outputs (artifacts).

### Inferred Critical Directories:
*   **`src/`:** (Core Source Directory) This directory likely contains all primary source code for the services and libraries. The `Dockerfile` context confirms this structure is used during build time (`WORKDIR "/src/MediaProcessorLibrary"`).
    *   **`MediaProcessorLibrary/`:** Highly probable location of core logic (the main `*.csproj`). This library encapsulates the foundational media processing functionality itself (contains business rules, utility classes, etc.).
*   **`services/`:** (Service Boundary Layer) Evidence suggests this boundary handles external communication, specifically mentioning gRPC services (`Services/MediaProcessorGrpcService.cs`). This layer is responsible for marshalling data between the outside world and the core logic through defined APIs.
*   **`models/`:** (Domain Models - Recommended) Based on industry best practices for layered C# applications, a dedicated `models/` area should house DTOs, business objects, and domain definitions used consistently across all services and libraries.

### 🚧 Build & Configuration Directives:
The `.gitignore` file reveals crucial directives for build management that agents must respect:
*   **Build Output Artifacts:** Directories like `Debug/`, `Release/`, `bin/`, `obj/` contain generated binaries, which are generally read-only by an agent performing analysis or execution role.
*   **Configuration:** Root level or subdirectories referenced in code (e.g., `appsettings.*`) are critical for connection strings and environment variables required by the agent's operational context.

## 🛠️ Key Dependencies & Technologies
The project is built upon a robust, modern technology stack:
1.  **.NET Core / .NET X+ (Inferred):** The foundational platform based on build tools and project file structures.
2.  **gRPC:** Essential for service-to-service communication and defining the public service contracts (`.proto` files). Agents must interact via these defined boundaries rather than direct code calls where appropriate.
3.  **Containerization (Docker):** The presence of a `Dockerfile` confirms that deployment requires containerization setup, meaning agents should be aware of build context and runtime images.

## 🧩 Core Functional Analysis & Agent Focus Areas
An agent designed for this system must prioritize these three areas:

1.  **Media Processing Logic Execution:** Interacting directly with the core business logic in `MediaProcessorLibrary/`. This is the operational heart of the system. (Input processing $\rightarrow$ Output stream generation).
2.  **API Interaction & Service Orchestration:** Defining and calling procedures via the gRPC interface to manage state or coordinate tasks across multiple microservices.
3.  **Environment Compliance:** Ensuring all actions adhere to the defined configuration model (`appsettings.*`) and that inputs undergo strict validation against expected data schemas (Domain Models).

## ✅ Agent Development Checklist:
*   [ ] **Knowledge Boundary:** The agent must understand that it is manipulating *compile-time structure* (source code) and treating build artifacts (`Debug/`, `bin/`) as immutable runtime resources.
*   [ ] **Input Validation:** All media input files MUST be validated against the defined object constraints (models).
*   [ ] **Build Dependency Awareness:** When performing deployments, the agent must correctly execute the build pipeline steps outlined in the Dockerfile context (`dotnet restore`, `dotnet build`) *before* running any code.

---
***End of Agent Analysis Documentation. Ready for implementation.***