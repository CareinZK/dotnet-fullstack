namespace TemplateService.Domain;

public class Position
{

    public Position(Guid id, string name)
    {
        if (id != Guid.Empty)
            this.Id = id;
        else       
        	throw new ArgumentException("Id cannot be empty.", nameof(id));
        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public Guid Id { get; private init; }

    public DateTime CreatedAt { get; private init; }

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

}