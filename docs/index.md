# Documentation Index

Welcome to the documentation for **dotnet-fullstack**, a .NET 10 Modular Monolith project built with Clean Architecture, Domain-Driven Design (DDD), custom CQRS vertical slices, the Result Pattern, and swappable persistence (EF Core and Dapper).

---

## Documentation Architecture (3 Tiers)

This repository adheres to 3-tier documentation structure:

1. **Tier 1 (Granular Documents - `docs/`)**: Detailed, modular technical guides organized into cross-cutting standards and module-specific bounded contexts.
2. **Tier 2 (The Map & Invariants - `llms.txt`)**: Root index containing architectural rules, technology matrix, and an intent-based file routing matrix.
3. **Tier 3 (Consolidated Corpus - `llms-full.txt`)**: Single-file bundled corpus of all documentation for single-pass LLM ingestion.

---

## Cross-Cutting Standards

- [Modular Monolith Architecture](architecture/overview.md): System layout, Clean Architecture layers, inter-module isolation rules, and design philosophies.
- [Getting Started & Development](development/getting-started.md): Prerequisites (.NET 10, Docker), Docker Compose services, build/run workflows, and Roslyn/Sonar static analyzer rules.
- [Testing Strategy](development/testing-strategy.md): xUnit test conventions, unit tests, slice handler tests, controller endpoint integration tests, and logging assertions.

---

## Modules Directory (`docs/modules/`)

The system is designed as a Modular Monolith. Each module represents an independent bounded context with its own internal Clean Architecture layers:

### Registered Modules

- **[DirectoryService](modules/directory-service/overview.md)**: Manages organizational structure, hierarchical departments (materialized paths), positions, locations, and primary office assignments.
  - [Domain Model](modules/directory-service/domain-model.md): Entities, aggregates, value invariants, factory methods, and the functional `Error` / `Result` pattern.
  - [CQRS Application Slices](modules/directory-service/cqrs-application.md): Command and query vertical slices (`ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`) and FluentValidation pipelines.
  - [Persistence & Repositories](modules/directory-service/persistence.md): Dual repository pattern (EF Core + Dapper), connection pooling via singleton `NpgsqlDataSource`, and PostgreSQL migrations.
  - [API & Presentation](modules/directory-service/api-presentation.md): ASP.NET Core 10 Web API, `Envelope<T>` response contracts, HTTP status code mapping, Scalar OpenAPI, and Serilog/Seq logging.

---

## Adding New Modules

When a new module is added to `backend/<ModuleName>`:
1. Create a corresponding documentation directory under `docs/modules/<module-slug>/`.
2. Document the module's domain model, use-case slices, persistence model, and API endpoints.
3. Register the module in `docs/index.md`, `llms.txt`, and rebuild `llms-full.txt` using `scripts/generate-llms-full.ps1`.
