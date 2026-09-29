# DirectoryService: CQRS Application Layer

## 1. Application Architecture

The **`DirectoryService.Application`** project implements use-case workflows using the **Command Query Responsibility Segregation (CQRS)** pattern organized into **Vertical Slices**.

### Design Philosophy
- **No MediatR**: Handlers are injected directly as strongly-typed `ICommandHandler<TCommand, TResponse>` or `IQueryHandler<TQuery, TResponse>`. This provides compile-time safety, zero reflection overhead at dispatch, and effortless navigation in IDEs.
- **Fail-Fast Validation**: Each command has an associated FluentValidation validator that executes before any database interaction.
- **Pure Result Returns**: Handlers return `Result<TResponse, ErrorList>` or `UnitResult<ErrorList>`. They never throw business exceptions.

---

## 2. CQRS Abstractions (`DirectoryService.Application.Common`)

```csharp
// Commands (State Changes)
public interface ICommand;
public interface ICommand<out TResponse>;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<ErrorList>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse, ErrorList>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

// Queries (Reads Only)
public interface IQuery<out TResponse>;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse, ErrorList>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
```

---

## 3. Vertical Slices Catalog

Each use-case is isolated into its own directory containing the command/query record, the validator, and the handler.

### Department Slices (`Departments/`)

| Slice Folder | Command / Query | Returns | Key Operations & Rules |
| :--- | :--- | :--- | :--- |
| **`CreateDepartment/`** | `CreateDepartmentCommand` | `DepartmentDto` | Validates name/slug uniqueness, validates parent department existence, builds materialized path, links optional locations. |
| **`GetDepartments/`** | `GetDepartmentsQuery` | `IReadOnlyList<DepartmentDto>` | Retrieves complete department tree. |
| **`GetDepartmentById/`** | `GetDepartmentByIdQuery` | `DepartmentDto` | Retrieves single department with linked locations and position IDs. |
| **`UpdateDepartment/`** | `UpdateDepartmentCommand` | `UnitResult` | Full update of department metadata and locations. |
| **`UpdateDepartmentName/`** | `UpdateDepartmentNameCommand` | `Guid` | Targeted rename slice; checks unique name, delegates to `Department.ChangeName`. |
| **`DeleteDepartment/`** | `DeleteDepartmentCommand` | `UnitResult` | Deletes department and cascades clean removal of associations. |
| **`LinkDepartmentLocation/`** | `LinkDepartmentLocationCommand` | `UnitResult` | Associates an existing location to a department, ensuring primary status consistency. |
| **`UnlinkDepartmentLocation/`** | `UnlinkDepartmentLocationCommand` | `UnitResult` | Removes location association from department. |

### Location Slices (`Locations/`)

| Slice Folder | Command / Query | Returns | Key Operations & Rules |
| :--- | :--- | :--- | :--- |
| **`CreateLocation/`** | `CreateLocationCommand` | `LocationDto` | Validates name uniqueness, creates `Location` entity. |
| **`GetLocations/`** | `GetLocationsQuery` | `IReadOnlyList<LocationDto>` | Lists all physical offices. |
| **`GetLocationById/`** | `GetLocationByIdQuery` | `LocationDto` | Fetches single location by Guid. |
| **`UpdateLocation/`** | `UpdateLocationCommand` | `UnitResult` | Updates name and address. |
| **`UpdateLocationName/`** | `UpdateLocationNameCommand` | `Guid` | Targeted location rename. |
| **`DeleteLocation/`** | `DeleteLocationCommand` | `UnitResult` | Deletes location if not referenced as active primary location. |

---

## 4. FluentValidation Integration

Validation is decoupled from controllers and encapsulated in command validators:

```csharp
public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Department name is required.")
            .MaximumLength(100).WithMessage("Department name must not exceed 100 characters.");

        RuleFor(c => c.Slug)
            .NotEmpty().WithMessage("Department slug is required.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Department slug must be URL-safe.");
    }
}
```

In handlers, errors are converted to the domain `ErrorList`:
```csharp
var validationResult = await _validator.ValidateAsync(command, cancellationToken);
if (!validationResult.IsValid)
{
    var errors = validationResult.ToErrorList();
    _logger.LogWarning("Validation failed: {@Errors}", errors);
    return errors;
}
```

---

## 5. Dependency Injection Registration

In `DirectoryService.Application/DependencyInjection.cs`:
- Automatically scans the assembly to register all `ICommandHandler` and `IQueryHandler` implementations.
- Registers all `IValidator<T>` implementations via `FluentValidation.DependencyInjectionExtensions`.
