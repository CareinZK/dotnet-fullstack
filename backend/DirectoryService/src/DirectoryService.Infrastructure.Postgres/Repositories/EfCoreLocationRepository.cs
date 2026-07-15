using DirectoryService.Application.Locations;
using DirectoryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

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

    public async Task<bool> AddLocationAsync(Location location, CancellationToken cancellationToken)
    {
        await _dbContext.Locations.AddAsync(location, cancellationToken);
        
        if (await NameExistsAsync(location.Name, cancellationToken))
        {
            _logger.LogError("Location with name '{Name}' already exists.", location.Name);
            return false;
        }

       
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}