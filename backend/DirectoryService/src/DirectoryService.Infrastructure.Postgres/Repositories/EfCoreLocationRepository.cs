using DirectoryService.Application.Locations;
using DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public sealed class EfCoreLocationRepository : ILocationRepository
{
 public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Locations
        .AnyAsync(l => l.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    private readonly AppDbContext _dbContext;
    private readonly ILogger<EfCoreLocationRepository> _logger;

    public EfCoreLocationRepository(AppDbContext dbContext, ILogger<EfCoreLocationRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;

    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);
        
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save location {LocationId} with name {Name}", location.Id, location.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Guid id, string name, string address, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Locations
            .Where(l => l.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Name, name)
                .SetProperty(l => l.Address, address)
                .SetProperty(l => l.UpdatedAt, DateTime.UtcNow), cancellationToken);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Locations
            .Where(l => l.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return rows > 0;
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Locations
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var location = await _dbContext.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        return location;
    }

    public async Task UpdateLocationNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        await _dbContext.Locations
            .Where(l => l.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.Name, name), cancellationToken);
    }
}
