using DirectoryService.Domain;

namespace DirectoryService.Application.Locations;

public sealed class LocationRepository : ILocationRepository
{
    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("LocationRepository is not yet implemented. This is a stub that will be replaced with a real implementation in the Infrastructure layer.");
    }

    public Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("LocationRepository is not yet implemented. This is a stub that will be replaced with a real implementation in the Infrastructure layer.");
    }
}
