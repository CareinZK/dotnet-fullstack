# DirectoryService: API & Presentation Layer

## 1. Presentation Architecture

The **`DirectoryService.Presentation`** layer exposes RESTful HTTP endpoints using **ASP.NET Core 10** Web API controllers.

### Core Responsibilities
- Maps incoming HTTP requests to CQRS Commands or Queries.
- Enforces a uniform response envelope (`Envelope<T>`).
- Maps domain and application `ErrorType` values to standard HTTP status codes.
- Instruments requests with structured Serilog logging and diagnostic telemetry.
- Hosts interactive API documentation via Scalar (`/scalar/v1`).

---

## 2. The Uniform Response Envelope

All endpoints return a standardized JSON envelope structure, eliminating client-side response ambiguity.

### Successful Response Format (`Envelope<T>`)
```json
{
  "result": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Backend Engineering",
    "slug": "backend-engineering",
    "parentId": null,
    "path": "backend-engineering",
    "locations": []
  },
  "errors": null,
  "timeGenerated": "2026-09-12T16:30:00Z"
}
```

### Error Response Format (`Envelope`)
```json
{
  "result": null,
  "errors": [
    {
      "code": "department.already.exists",
      "message": "Department with name 'Backend Engineering' already exists.",
      "type": "Conflict",
      "invalidField": null
    }
  ],
  "timeGenerated": "2026-09-12T16:30:00Z"
}
```

---

## 3. Result-to-HTTP Status Code Mapping

Controllers delegate directly to `ResultExtensions`:

```csharp
// In controller action:
var result = await _createDepartmentHandler.Handle(command, cancellationToken);
return result.ToCreatedEnvelopeResult(); // Returns 201 Created on success
```

The mapping function `ResultExtensions.GetStatusCode` evaluates the primary error:

| Domain `ErrorType` | HTTP Status Code | Scenario |
| :--- | :--- | :--- |
| **`Validation`** | **`400 Bad Request`** | Input syntax, empty fields, regex mismatch, rule violations. |
| **`Unauthorized`** | **`401 Unauthorized`** | Missing or invalid authentication token. |
| **`Forbidden`** | **`403 Forbidden`** | Insufficient permissions to perform operation. |
| **`NotFound`** | **`404 Not Found`** | Requested entity ID or slug does not exist. |
| **`Conflict`** | **`409 Conflict`** | Duplicate name/slug, foreign key collisions, unique constraints. |
| **`Failure`** | **`500 Internal Server Error`** | Unhandled database error or unexpected system failure. |

---

## 4. API Endpoints Catalog

### Departments (`/departments`)

| Method | Route | Description | Request Body | Success Status | Error Statuses |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `POST` | `/departments` | Create a new department | `CreateDepartmentDto` | `201 Created` | 400, 404, 409 |
| `GET` | `/departments` | List all departments | _None_ | `200 OK` | 500 |
| `GET` | `/departments/{id}` | Get department by ID | _None_ | `200 OK` | 404 |
| `PUT` | `/departments/{id}` | Full update of department | `UpdateDepartmentDto` | `200 OK` | 400, 404, 409 |
| `PATCH` | `/departments/{id}/name` | Rename department | `UpdateDepartmentNameRequest` | `200 OK` | 400, 404, 409 |
| `DELETE` | `/departments/{id}` | Delete department | _None_ | `200 OK` | 404 |

### Department Locations (`/departments/{departmentId}/locations`)

| Method | Route | Description | Request Body | Success Status |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/departments/{deptId}/locations/{locId}` | Link office location to department | `LinkDepartmentLocationRequest` | `200 OK` |
| `DELETE` | `/departments/{deptId}/locations/{locId}` | Remove office link from department | _None_ | `200 OK` |

### Locations (`/locations`)

| Method | Route | Description | Request Body | Success Status |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/locations` | Create a physical location | `CreateLocationDto` | `201 Created` |
| `GET` | `/locations` | List all physical locations | _None_ | `200 OK` |
| `GET` | `/locations/{id}` | Get location by ID | _None_ | `200 OK` |
| `PUT` | `/locations/{id}` | Update location details | `UpdateLocationDto` | `200 OK` |
| `PATCH` | `/locations/{id}/name` | Rename location | `UpdateLocationNameRequest` | `200 OK` |
| `DELETE` | `/locations/{id}` | Delete location | _None_ | `200 OK` |

---

## 5. Observability & Logging

- **Serilog**: Configured with console sink, Seq sink, and enrichment:
  - `MachineName`, `EnvironmentName`, `ServiceName ("DirectoryService")`.
  - Destructures entity framework database update exceptions (`DbUpdateExceptionDestructurer`).
- **Diagnostic Context**:
  ```csharp
  if (result.IsSuccess)
  {
      _diagnosticContext?.Set("DepartmentId", result.Value.Id);
  }
  ```
- **Global Exception Handler**: `GlobalExceptionHandler` intercepts unhandled exceptions, logs fatal errors, and produces an RFC 7807 `ProblemDetails` response with HTTP 500.

---

## 6. Interactive OpenAPI Documentation (Scalar)

Scalar is configured in `Program.cs` for development environments:
- Interactive UI: `http://localhost:5000/scalar/v1`
- OpenAPI v3 Specification: `http://localhost:5000/openapi/v1.json`
