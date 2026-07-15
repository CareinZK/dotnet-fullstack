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


}