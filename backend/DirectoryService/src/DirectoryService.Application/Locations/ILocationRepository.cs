using DirectoryService.Domain;

namespace DirectoryService.Application.Locations;

public interface ILocationRepository
{
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Location location, CancellationToken cancellationToken);
    Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Location location, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateLocationNameAsync(Guid id, string name, CancellationToken cancellationToken);
}
