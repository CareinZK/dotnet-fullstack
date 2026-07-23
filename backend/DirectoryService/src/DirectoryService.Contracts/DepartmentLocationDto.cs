namespace DirectoryService.Contracts;

public sealed record DepartmentLocationDto(Guid id, Guid departmentId, Guid locationId, bool isPrimaryLocation);

public sealed record CreateDepartmentLocationDto(Guid id, Guid departmentId, Guid locationId, bool isPrimaryLocation);

public sealed record UpdateDepartmentLocationDto(Guid id, Guid departmentId, Guid locationId, bool isPrimaryLocation);

public interface IDepartmentLocationsService
{
    Task<DepartmentLocationDto> CreateAsync(CreateDepartmentLocationDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<DepartmentLocationDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentLocationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateDepartmentLocationDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}