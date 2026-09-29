# DirectoryService Module Overview

## 1. Module Purpose & Bounded Context

The **`DirectoryService`** module is the core organizational directory bounded context within the Modular Monolith. It governs:
- **Departments**: Tree-structured organizational units organized via hierarchical materialized paths (e.g., `engineering/backend`).
- **Locations**: Physical company offices and work sites.
- **Positions**: Standardized job titles and organizational roles.
- **Department-Location Associations**: Office assignments for departments, including tracking of each department's primary office (`IsPrimaryLocation`).
- **Department-Position Associations**: Mapping of valid job titles applicable to specific departments.

---

## 2. Solution Structure

The module lives under `backend/DirectoryService/` and consists of 5 internal Clean Architecture projects:

```
backend/DirectoryService/
├── DirectoryService.slnx
├── docs/                                  # Internal snapshots and legacy notes
├── src/
│   ├── DirectoryService.Domain/           # Entities, Aggregates, Invariants, Error Catalog
│   ├── DirectoryService.Contracts/        # Public Request/Response DTOs
│   ├── DirectoryService.Application/      # CQRS Slices, Handlers, Validators, Repository Interfaces
│   ├── DirectoryService.Infrastructure.Postgres/ # EF Core DbContext, Dapper Repos, Migrations
│   └── DirectoryService.Presentation/     # Controllers, ResultExtensions, Envelopes, Program.cs
└── tests/
    └── DirectoryService.Tests/            # xUnit test suite (Domain, Handlers, Endpoints, DI)
```

---

## 3. Subsystem Invariants & Key Rules

1. **Hierarchy Integrity**:
   - Every department has an immutable `Slug` and a calculated `Path` (materialized path).
   - If a department has a parent, its path is `{Parent.Path}/{Slug}`. Root departments have `Path = {Slug}`.
   - Slugs must be URL-safe (lowercase letters, digits, hyphens).
2. **Primary Location Constraint**:
   - A department can be linked to multiple locations, but only one location can be marked as `IsPrimaryLocation = true`.
3. **Dual Persistence Support**:
   - Both Entity Framework Core and Dapper are first-class persistence implementations.
   - All repository methods return functional `Result<T, Error>` or `UnitResult<Error>` objects.
4. **Uniform Response Envelope**:
   - All HTTP endpoints wrap responses in an `Envelope<T>` object containing the timestamp, result payload, and structured error list.
