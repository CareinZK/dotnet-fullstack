using DirectoryService.Domain;

namespace DirectoryService.Application.Locations;

public interface ILocationRepository
{
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Location location, CancellationToken cancellationToken);
}
