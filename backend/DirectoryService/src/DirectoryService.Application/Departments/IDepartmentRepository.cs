using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments;

public interface IDepartmentRepository
{
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Department department, IReadOnlyCollection<Guid> locationIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Department department, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateDepartmentNameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task AddLocationLinkAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<bool> RemoveLocationLinkAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}
