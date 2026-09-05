using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Contracts;

public sealed record DepartmentLocationDto(Guid Id, Guid DepartmentId, Guid LocationId, bool IsPrimaryLocation);

public sealed record CreateDepartmentLocationDto(Guid Id, Guid DepartmentId, Guid LocationId, bool IsPrimaryLocation);

public sealed record UpdateDepartmentLocationDto(Guid Id, Guid DepartmentId, Guid LocationId, bool IsPrimaryLocation);

// ReSharper disable UnusedMember.Global
// ReSharper disable once UnusedType.Global
public interface IDepartmentLocationsService
{
    Task<Result<DepartmentLocationDto, ErrorList>> CreateAsync(CreateDepartmentLocationDto dto, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<DepartmentLocationDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<DepartmentLocationDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdateDepartmentLocationDto dto, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}