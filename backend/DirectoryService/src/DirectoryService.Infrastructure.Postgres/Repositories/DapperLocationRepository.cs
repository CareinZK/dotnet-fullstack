using System.Data;
using Dapper;
using DirectoryService.Application.Locations;
using DirectoryService.Domain;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class DapperLocationRepository : ILocationRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger<DapperLocationRepository> _logger;

    public DapperLocationRepository(IDbConnection connection, ILogger<DapperLocationRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT EXISTS(
        SELECT 1
        FROM locations
        WHERE lower(name) = lower(@Name))
        """;
        return await _connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Name = name }, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO locations (id, name, address, created_at, updated_at)
        VALUES (@Id, @Name, @Address, @CreatedAt, @UpdatedAt)
        """;
        try
        {
            await _connection.ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                CreatedAt = location.CreatedAt,
                UpdatedAt = location.UpdatedAt
            }, cancellationToken: cancellationToken));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save location {LocationId} with name {Name}", location.Id, location.Name);
            throw;
        }
    }

}
