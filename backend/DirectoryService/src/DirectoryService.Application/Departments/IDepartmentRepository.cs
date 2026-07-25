using DirectoryService.Domain.Departments;

namespace DirectoryService.Application.Departments;

public interface IDepartmentRepository
{
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken);
    Task AddAsync(Department department, IReadOnlyCollection<Guid> locationIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, string name, string address, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateDepartmentNameAsync(Guid id, string name, CancellationToken cancellationToken);
}
