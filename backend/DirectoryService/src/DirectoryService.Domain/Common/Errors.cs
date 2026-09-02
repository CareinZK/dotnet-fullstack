namespace DirectoryService.Domain.Common;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsRequired(string? fieldName = null) =>
            Error.Validation("value.is.required", string.IsNullOrWhiteSpace(fieldName) ? "Value is required." : $"{fieldName} is required.", fieldName);

        public static Error ValueIsInvalid(string? fieldName = null, string? message = null) =>
            Error.Validation("value.is.invalid", message ?? (string.IsNullOrWhiteSpace(fieldName) ? "Value is invalid." : $"{fieldName} is invalid."), fieldName);

        public static Error NotFound(Guid? id = null, string? name = null)
        {
            if (id.HasValue)
            {
                return Error.NotFound("record.not.found", $"Record with id '{id.Value}' was not found.");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                return Error.NotFound("record.not.found", $"Record '{name}' was not found.");
            }

            return Error.NotFound("record.not.found", "Record was not found.");
        }

        public static Error Failure(string? message = null) =>
            Error.Failure("internal.server.error", message ?? "An unexpected error occurred.");

        public static Error Database(string? message = null) =>
            Error.Failure("database.error", message ?? "A database error occurred.");
    }

    public static class Location
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("location.not.found", $"Location with id '{id}' was not found.");

        public static Error AlreadyExists(string name) =>
            Error.Conflict("location.already.exists", $"Location with name '{name}' already exists.");
    }

    public static class Department
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("department.not.found", $"Department with id '{id}' was not found.");

        public static Error AlreadyExists(string name) =>
            Error.Conflict("department.already.exists", $"Department with name '{name}' already exists.");

        public static Error ParentNotFound(Guid parentId) =>
            Error.NotFound("department.parent.not.found", $"Parent department with id '{parentId}' was not found.");

        public static Error LocationAlreadyLinked(Guid departmentId, Guid locationId) =>
            Error.Conflict("department.location.already.linked", $"Location '{locationId}' is already linked to department '{departmentId}'.");

        public static Error LocationNotLinked(Guid departmentId, Guid locationId) =>
            Error.NotFound("department.location.not.linked", $"Location '{locationId}' is not linked to department '{departmentId}'.");
    }

    public static class Position
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound("position.not.found", $"Position with id '{id}' was not found.");

        public static Error AlreadyExists(string name) =>
            Error.Conflict("position.already.exists", $"Position with name '{name}' already exists.");
    }
}
