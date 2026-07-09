namespace TemplateService.Domain.Departments;
using System.Text.RegularExpressions;

public class Department
{
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

	public Guid Id { get; init; }
    
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

    public string Slug { get; init; }
    
    private static readonly Regex SlugRegex = new(
        "^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1));
	
    public string Path { get; init; }
    
    public DateTime CreatedAt { get; private set;}
    
    public DateTime UpdatedAt { get; private set; }


}