namespace DirectoryService.Contracts;

public sealed record DepartmentDto(Guid Id, string Name, string Slug, string Path, Guid? ParentId, List<Guid>? LocationIds);

public sealed record CreateDepartmentDto(string Name, string Slug, Guid? ParentId, List<Guid>? LocationIds);

public sealed record UpdateDepartmentDto(string Name);

public interface IDepartmentsService
{
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task LinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task UnlinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}
