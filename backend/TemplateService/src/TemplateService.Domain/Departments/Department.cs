namespace TemplateService.Domain.Departments;

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
        if (string.IsNullOrWhiteSpace(slug))
			throw new ArgumentException("Department slug cannot be empty.", nameof(slug));    
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
	
    public string Path { get; init; }
    
    public DateTime CreatedAt { get; private set;}
    
    public DateTime UpdatedAt { get; private set; }


}