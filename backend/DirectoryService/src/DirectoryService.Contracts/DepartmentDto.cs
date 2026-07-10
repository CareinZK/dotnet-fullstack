namespace DirectoryService.Contracts;

public sealed record DepartmentDto(Guid Id, string Name, string Slug, Guid? ParentId);

public sealed record CreateDepartmentDto(string Name, string Slug, Guid? ParentId);

public sealed record UpdateDepartmentDto(string Name, string Slug, Guid? ParentId);

public interface IDepartmentsService
{
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}