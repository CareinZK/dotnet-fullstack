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
        WHERE lower(name) = lower(@name))
        """;
        return await _connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Name = name }, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO locations (name, description)
        VALUES (@Name, @Description)
        """;
        await _connection.ExecuteAsync(new CommandDefinition(sql, location, cancellationToken: cancellationToken));
    }

}
