namespace DirectoryService.Domain.Departments;

using System.Text.RegularExpressions;

public class Department
{
    private const char PathSeparator = '/';
    private static readonly Regex SlugRegex = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));

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
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Department name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(slug) || !SlugRegex.IsMatch(slug))
        {
            throw new ArgumentException("Department slug must be a non-empty URL-safe string (lowercase letters, digits, hyphens).", nameof(slug));
        }

        Id = id;
        Name = name;
        Slug = slug;
        ParentId = parentDepartment?.Id;
        Path = BuildPath(parentDepartment, slug);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static Department Create(Guid id, string name, string slug, Department? parentDepartment)
    {
        return new Department(id, name, slug, parentDepartment);
    }

    public Guid Id { get; private set; }

    public Guid? ParentId { get; private set; }

    public string Name { get; private set; }

    public string Slug { get; private set; }

    public string Path { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private static string BuildPath(Department? parentDepartment, string slug)
    {
        return parentDepartment is null
            ? slug
            : string.Join(PathSeparator, parentDepartment.Path, slug);
    }
}