# Getting Started & Development Workflows

This document outlines the local development environment setup, infrastructure dependencies, build procedures, and coding standards.

---

## 1. Prerequisites

- **.NET 10 SDK** (Version `10.0.203` or later, as pinned in `global.json`).
- **Docker & Docker Compose** (for PostgreSQL and Seq containers).
- **PowerShell 7+** / Bash terminal.
- Recommended IDE: Visual Studio 2026, JetBrains Rider 2025+, or VS Code with C# Dev Kit.

---

## 2. Infrastructure Services (Docker Compose)

The repository provides local infrastructure dependencies via Docker Compose located at `backend/DirectoryService/src/docker-compose.yml`.

### Services

| Service | Image | Host Port | Internal Port | Description |
| :--- | :--- | :--- | :--- | :--- |
| **PostgreSQL** | `postgres:latest` | **`5434`** | `5432` | Relational database. Port is mapped to `5434` to prevent collisions with local default PostgreSQL instances. |
| **Seq** | `datalust/seq:latest` | **`5341`** | `80` | Real-time structured log ingestion and UI dashboard. |

### Environment Configuration (`.env`)

Environment variables reside in `backend/DirectoryService/src/.env`:
```env
POSTGRES_DB=directory_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
```

### Starting Infrastructure

```powershell
# From repository root
docker compose -f backend/DirectoryService/src/docker-compose.yml up -d
```

Verify containers are running:
```powershell
docker ps --filter "name=postgres" --filter "name=seq"
```

To stop containers:
```powershell
docker compose -f backend/DirectoryService/src/docker-compose.yml down
```

---

## 3. Database Migrations

Entity Framework Core migrations are located in `DirectoryService.Infrastructure.Postgres/Migrations/`.

To apply pending migrations to the local PostgreSQL database:
```powershell
dotnet ef database update `
  --project backend/DirectoryService/src/DirectoryService.Infrastructure.Postgres `
  --startup-project backend/DirectoryService/src/DirectoryService.Presentation
```

To add a new migration when domain entities or configurations change:
```powershell
dotnet ef migrations add <MigrationName> `
  --project backend/DirectoryService/src/DirectoryService.Infrastructure.Postgres `
  --startup-project backend/DirectoryService/src/DirectoryService.Presentation `
  --output-dir Migrations
```

---

## 4. Building and Running Locally

### Build Solution
```powershell
dotnet build backend/DirectoryService/DirectoryService.slnx
```

### Run Web API
```powershell
dotnet run --project backend/DirectoryService/src/DirectoryService.Presentation
```

### Endpoints & Observability
Once started, the application exposes:
- **Root**: `http://localhost:5000/` (returns `"Hello World!"`)
- **Health Check**: `http://localhost:5000/health` (EF Core database connectivity check)
- **Scalar OpenAPI Reference**: `http://localhost:5000/scalar/v1` (interactive API documentation)
- **OpenAPI Document**: `http://localhost:5000/openapi/v1.json`
- **Seq Log Dashboard**: `http://localhost:5341`

---

## 5. Configuration Settings

Settings are configured via `appsettings.json` and `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5434;Database=directory_db;Username=postgres;Password=postgres;"
  },
  "Repository": {
    "Implementation": "EFCore" // Options: "EFCore" or "Dapper"
  },
  "Seq": {
    "ServerUrl": "http://localhost:5341"
  }
}
```

- **Swapping Persistence**: Set `"Repository": { "Implementation": "Dapper" }` to switch the entire data layer to Dapper SQL repositories at runtime.

---

## 6. Static Analysis & Code Quality Rules

`backend/Directory.Build.props` automatically enforces code style and analyzer rules during every build:
- **Roslynator** (`Roslynator.Analyzers`): Universal C# conventions.
- **Meziantou** (`Meziantou.Analyzer`): Correctness, thread safety, and string culture rules.
- **AsyncFixer** (`AsyncFixer`): Eliminates async/await anti-patterns.
- **SonarAnalyzer** (`SonarAnalyzer.CSharp`): Security, reliability, and code smells.

To check for analyzer warnings across the solution:
```powershell
dotnet build backend/DirectoryService/DirectoryService.slnx /warnaserror:false
```
