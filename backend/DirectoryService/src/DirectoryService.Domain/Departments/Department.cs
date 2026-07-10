namespace DirectoryService.Domain.Departments;
using System.Text.RegularExpressions;

public class Department
{
	// Parameterless constructor for EF Core materialization
	private Department()
	{
		Name = string.Empty;
		Slug = string.Empty;
		Path = string.Empty;
		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
	}

	public Department(Guid id, string name, string slug, Department? parentDepartment)
	{
        if (id != Guid.Empty)
			this.Id = id;
        else       
        	throw new ArgumentException("Id cannot be empty.", nameof(id));
		Name = name;
        ParentId = parentDepartment?.Id;
	if (string.IsNullOrWhiteSpace(slug) || !SlugRegex.IsMatch(slug))
    	throw new ArgumentException("Department slug must be a non-empty URL-safe string (lowercase letters, digits, hyphens).", nameof(slug)); 
	Slug = slug;
        
		Path = parentDepartment is null
    	? slug : string.Join(PathSeparator, parentDepartment.Path, slug);
        
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
	}

	public Guid Id { get; private set; }

    public Guid? ParentId { get; private set; }

	 private const char PathSeparator = '/';
public string Name
{
    get;
    set
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Department name cannot be empty.", nameof(value));

        field = value;
        UpdatedAt = DateTime.UtcNow;
    }
}

    public string Slug { get; private set; }
    
    private static readonly Regex SlugRegex = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));
	
    public string Path { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime UpdatedAt { get; private set; }


}