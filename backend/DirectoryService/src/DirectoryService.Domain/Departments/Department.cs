using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private const char PathSeparator = '/';
    private static readonly Regex SlugRegex = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

    // ReSharper disable once UnusedMember.Local
    private Department()
    {
        Name = string.Empty;
        Slug = string.Empty;
        Path = string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Department(Guid id, string name, string slug, Department? parentDepartment)
    {
        Id = id;
        Name = name;
        Slug = slug;
        ParentId = parentDepartment?.Id;
        Path = BuildPath(parentDepartment, slug);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static Result<Department, Error> Create(Guid id, string name, string slug, Department? parentDepartment)
    {
        if (id == Guid.Empty)
        {
            return Error.Validation("department.id.invalid", "Id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("department.name.invalid", "Department name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug) || !SlugRegex.IsMatch(slug))
        {
            return Error.Validation("department.slug.invalid", "Department slug must be a non-empty URL-safe string (lowercase letters, digits, hyphens).", nameof(slug));
        }

        return new Department(id, name.Trim(), slug.Trim(), parentDepartment);
    }

    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
    public Guid Id { get; private set; }

    public Guid? ParentId { get; private set; }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    public string Path { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public UnitResult<Error> ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("department.name.invalid", "Department name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<Error>();
    }

    private static string BuildPath(Department? parentDepartment, string slug)
    {
        return parentDepartment is null
            ? slug
            : string.Join(PathSeparator, parentDepartment.Path, slug);
    }
}
