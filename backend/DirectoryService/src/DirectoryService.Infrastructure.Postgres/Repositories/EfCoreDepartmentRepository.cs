using DirectoryService.Application.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public sealed class EfCoreDepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<EfCoreDepartmentRepository> _logger;

    public EfCoreDepartmentRepository(AppDbContext dbContext, ILogger<EfCoreDepartmentRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AnyAsync(d => d.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task AddAsync(Department department, IReadOnlyCollection<Guid> locationIds, CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            foreach (var locationId in locationIds.Distinct())
            {
                var departmentLocation = new DepartmentLocation(
                    Guid.NewGuid(),
                    department.Id,
                    locationId,
                    isPrimaryLocation: false);

                await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save department {DepartmentId} with name {Name}", department.Id, department.Name);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string address, CancellationToken cancellationToken)
    {
        return false;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Departments
            .Where(d => d.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return rows > 0;
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        return department;
    }

    public async Task UpdateDepartmentNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        await _dbContext.Departments
            .Where(d => d.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.Name, name), cancellationToken);
    }
}
