using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Domain;

public class Position
{
    // ReSharper disable once UnusedMember.Local
    private Position()
    {
        Name = string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Position(Guid id, string name)
    {
        Id = id;
        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public static Result<Position, Error> Create(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            return Error.Validation("position.id.invalid", "Id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("position.name.invalid", "Position name cannot be empty.", nameof(name));
        }

        return new Position(id, name.Trim());
    }

    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string Name { get; private set; }

    // ReSharper disable once UnusedMember.Global
    public UnitResult<Error> ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("position.name.invalid", "Position name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Success<Error>();
    }
}