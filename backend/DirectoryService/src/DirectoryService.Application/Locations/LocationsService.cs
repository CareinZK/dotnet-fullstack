using DirectoryService.Contracts;
using DirectoryService.Application.Exceptions;
using DirectoryService.Domain;

namespace DirectoryService.Application.Locations;

public sealed class LocationsService : ILocationsService
{
    private readonly ILocationRepository _locationRepository;
    private readonly CreateLocation _createLocation;

    public LocationsService(ILocationRepository locationRepository, CreateLocation createLocation)
    {
        _locationRepository = locationRepository;
        _createLocation = createLocation;
    }

    public async Task<LocationDto> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken)
    {
        var id = await _createLocation.ExecuteAsync(dto, cancellationToken);
        var location = await _locationRepository.GetByIdAsync(id, cancellationToken);

        if (location is null)
        {
            throw new InvalidOperationException("Created location was not found after persistence.");
        }

        return Map(location);
    }

    public async Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetAllAsync(cancellationToken);
        return locations.Select(Map).ToList();
    }

    public async Task<LocationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(id, cancellationToken);
        return location is null ? null : Map(location);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (location is null)
        {
            return false;
        }

        location.UpdateDetails(dto.Name, dto.Address);
        return await _locationRepository.UpdateAsync(location, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _locationRepository.DeleteAsync(id, cancellationToken);
    }

    private static LocationDto Map(Location location) =>
        new(location.Id, location.Name, location.Address);
}
