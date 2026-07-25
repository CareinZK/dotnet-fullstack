namespace DirectoryService.Domain;

public class Location
{
    private Location()
    {
        Name = string.Empty;
        Address = string.Empty;
    }

    public Location(Guid id, string name, string address)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));

        Id = id;
        ChangeName(name);
        ChangeAddress(address);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty.", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Location address cannot be empty.", nameof(address));

        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Location name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Location address cannot be empty.", nameof(address));

        Name = name;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
}
