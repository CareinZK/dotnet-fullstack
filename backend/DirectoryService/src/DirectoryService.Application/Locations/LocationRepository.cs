using DirectoryService.Domain;

namespace DirectoryService.Application.Locations;

public sealed class LocationRepository : ILocationRepository
{
    public Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
#pragma warning disable MA0025 // Implement the functionality instead of throwing NotImplementedException

        throw new NotImplementedException("LocationRepository is not yet implemented. This is a stub that will be replaced with a real implementation in the Infrastructure layer.");
#pragma warning restore MA0025 // Implement the functionality instead of throwing NotImplementedException

    }

    public Task AddAsync(Location location, CancellationToken cancellationToken)
    {
#pragma warning disable MA0025 // Implement the functionality instead of throwing NotImplementedException

        throw new NotImplementedException("LocationRepository is not yet implemented. This is a stub that will be replaced with a real implementation in the Infrastructure layer.");
#pragma warning restore MA0025 // Implement the functionality instead of throwing NotImplementedException

    }
}
