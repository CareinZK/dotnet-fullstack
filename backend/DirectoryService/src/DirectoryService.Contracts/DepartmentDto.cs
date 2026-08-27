using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Contracts;

public sealed record DepartmentDto(Guid Id, string Name, string Slug, string Path, Guid? ParentId, List<Guid>? LocationIds);

public sealed record CreateDepartmentDto(string Name, string Slug, Guid? ParentId, List<Guid>? LocationIds);

public sealed record UpdateDepartmentDto(string Name);

public interface IDepartmentsService
{
    Task<Result<DepartmentDto, ErrorList>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<DepartmentDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<DepartmentDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> LinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UnlinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}
