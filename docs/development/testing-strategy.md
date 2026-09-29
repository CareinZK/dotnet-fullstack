# Testing Strategy & Verification Suite

This document describes the testing architecture, test suites, execution commands, and guidelines for maintaining high test coverage across modules.

---

## 1. Overview

Testing is centralized under `backend/<ModuleName>/tests/`. For `DirectoryService`, all tests reside in `backend/DirectoryService/tests/DirectoryService.Tests/`.

The test suite uses:
- **xUnit** (`xunit 2.9.3`) as the test runner.
- Strongly-typed assertions verifying both successful domain values and `ErrorList` contents.
- Fast in-memory execution (87 tests executing in ~2 seconds).

---

## 2. Test Suite Breakdown

The test project contains 8 targeted test files covering every architectural layer:

| Test File | Layer Tested | Primary Responsibilities |
| :--- | :--- | :--- |
| **`DomainTests.cs`** | Domain | Verifies entity creation (`Department.Create`, `Location.Create`, `Position.Create`), invariants (empty Guids, null/whitespace names, invalid slugs), materialized path calculation (`BuildPath`), and rename methods (`ChangeName`). |
| **`HandlerTests.cs`** | Application | Tests CQRS command and query handlers (`CreateDepartmentHandler`, `DeleteDepartmentHandler`, `GetDepartmentByIdHandler`, `GetDepartmentsHandler`, etc.) using mocked repository dependencies. Verifies business validation, duplicate checks, and proper `Result` / `ErrorList` returns. |
| **`ValidationTests.cs`** | Application | Tests FluentValidation validators (e.g., `CreateDepartmentCommandValidator`) against boundary conditions, length constraints, and slug regex matching. |
| **`ControllerEndpointTests.cs`** | Presentation | Tests ASP.NET Core controllers (`DepartmentsController`, `LocationController`, etc.). Validates correct delegation to CQRS handlers, HTTP status code mapping (200, 201, 400, 404, 409), and `Envelope<T>` serialization. |
| **`ResultMappingTests.cs`** | Presentation | Verifies `ResultExtensions.GetStatusCode` mappings: `ErrorType.Validation` → 400, `NotFound` → 404, `Conflict` → 409, `Forbidden` → 403, `Unauthorized` → 401, `Failure` → 500. |
| **`LoggingTests.cs`** | Presentation / App | Validates Serilog structured logging enrichment, Serilog exception destructurers (`DbUpdateExceptionDestructurer`), and `IDiagnosticContext` parameter setting. |
| **`DependencyInjectionTests.cs`** | Infrastructure / Setup | Ensures all services, handlers, validators, and repositories register correctly in the Microsoft DI container. Specifically tests runtime swapping between `EFCore` and `Dapper` configurations. |
| **`RepositoryCancellationTests.cs`** | Infrastructure | Asserts that `CancellationToken` is properly honored by repository asynchronous methods and that `OperationCanceledException` is propagated rather than swallowed or converted to database errors. |

---

## 3. Running Tests

### Run All Tests
```powershell
dotnet test backend/DirectoryService/DirectoryService.slnx
```

### Run a Specific Test Class
```powershell
dotnet test backend/DirectoryService/DirectoryService.slnx --filter FullyQualifiedName~HandlerTests
```

### Run with Detailed Output
```powershell
dotnet test backend/DirectoryService/DirectoryService.slnx --logger "console;verbosity=detailed"
```

---

## 4. Testing Conventions for New Features

When creating a new CQRS vertical slice or adding a module:

1. **Domain Unit Tests**: Test factory methods (`Create`) with both valid data and invalid edge cases (empty strings, invalid formats, invalid IDs).
2. **Validator Tests**: Ensure each validation rule in the `AbstractValidator<TCommand>` has matching test assertions.
3. **Handler Tests**:
   - Mock all repository dependencies (`IDepartmentRepository`, etc.).
   - Test the happy path returning `Result.Success`.
   - Test every validation failure and business failure path (e.g. duplicate name, entity not found) returning appropriate `ErrorList` entries.
4. **Endpoint Tests**:
   - Assert that handlers are invoked with the mapped command.
   - Assert the HTTP response envelope format (`Envelope<T>`) and HTTP status code.
