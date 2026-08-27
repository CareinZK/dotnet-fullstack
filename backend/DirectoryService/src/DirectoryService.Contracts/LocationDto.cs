using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Contracts;

public sealed record LocationDto(Guid Id, string Name, string Address);

public sealed record CreateLocationDto(string Name, string Address);

public sealed record UpdateLocationDto(string Name, string Address);

public interface ILocationsService
{
    Task<Result<LocationDto, ErrorList>> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<LocationDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<LocationDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}