namespace DirectoryService.Contracts;

public sealed record LocationDto(Guid Id, string Name, string Address);

public sealed record CreateLocationDto(string Name, string Address);

public sealed record UpdateLocationDto(string Name, string Address);

public interface ILocationsService
{
    Task<LocationDto> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<LocationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}