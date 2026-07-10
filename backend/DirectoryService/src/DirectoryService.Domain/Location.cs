namespace TemplateService.Domain;

public class Location
{
	// Parameterless constructor for EF Core materialization
	private Location()
	{
		Name = string.Empty;
		Address = string.Empty;
		CreatedAt = DateTime.UtcNow;
		UpdatedAt = DateTime.UtcNow;
	}

      public Location(Guid id, string name, string address)
    {
        if (id != Guid.Empty)
            this.Id = id;
        else       
        	throw new ArgumentException("Id cannot be empty.", nameof(id));
        Name = name;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public Guid Id { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }


    public string Name
{
    get;
    set
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Location name cannot be empty.", nameof(value));

        field = value;
        UpdatedAt = DateTime.UtcNow;
    }
}

    public string Address
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Location address cannot be empty.", nameof(value));

            field = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
