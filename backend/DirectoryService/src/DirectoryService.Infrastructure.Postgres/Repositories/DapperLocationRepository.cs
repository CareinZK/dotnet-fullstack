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

    public async Task<bool> UpdateAsync(Location location, CancellationToken cancellationToken)
    {
        const string updateSql = """
        UPDATE locations
        SET name = @Name, address = @Address, updated_at = @UpdatedAt
        WHERE id = @Id
        """;

        var rows = await _connection.ExecuteAsync(
            new CommandDefinition(
                updateSql,
                new { location.Id, location.Name, location.Address, location.UpdatedAt },
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        const string deleteSql = """
        DELETE FROM locations
        WHERE id = @Id
        """;

        var rows = await _connection.ExecuteAsync(
            new CommandDefinition(
                deleteSql,
                new { Id = id },
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<IReadOnlyList<Location>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, name, address, created_at AS CreatedAt, updated_at AS UpdatedAt
        FROM locations
        """;

        var locations = await _connection.QueryAsync<Location>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return locations.AsList();
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, name, address, created_at AS CreatedAt, updated_at AS UpdatedAt
        FROM locations
        WHERE id = @Id
        """;

        return await _connection.QuerySingleOrDefaultAsync<Location>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task UpdateLocationNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {

        const string updateNameSql = """
        UPDATE locations
        SET name = @Name, updated_at = NOW()
        WHERE id = @Id
        """;

        try
        {
            await _connection.ExecuteAsync(
                new CommandDefinition(
                    updateNameSql,
                    new { Id = id, Name = name },
                    cancellationToken: cancellationToken));
   
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update location name to {Name}", name);
            throw;
        }
    }
}
