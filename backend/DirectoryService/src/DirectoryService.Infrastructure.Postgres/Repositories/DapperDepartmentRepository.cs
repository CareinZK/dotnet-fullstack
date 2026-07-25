using System.Data;
using Dapper;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public sealed class DapperDepartmentRepository : IDepartmentRepository
{
    private readonly IDbConnection _connection;
    private readonly ILogger<DapperDepartmentRepository> _logger;

    public DapperDepartmentRepository(IDbConnection connection, ILogger<DapperDepartmentRepository> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT EXISTS(
            SELECT 1
            FROM departments
            WHERE lower(name) = lower(@Name)
        )
        """;

        return await _connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Name = name }, cancellationToken: cancellationToken));
    }

    public async Task AddAsync(Department department, IReadOnlyCollection<Guid> locationIds, CancellationToken cancellationToken)
    {
        const string departmentSql = """
        INSERT INTO departments (id, name, slug, path, parent_id, created_at, updated_at)
        VALUES (@Id, @Name, @Slug, @Path, @ParentId, @CreatedAt, @UpdatedAt)
        """;

        const string departmentLocationSql = """
        INSERT INTO department_locations (id, department_id, location_id, is_primary_location)
        VALUES (@Id, @DepartmentId, @LocationId, @IsPrimaryLocation)
        """;

        var transaction = _connection.BeginTransaction();

        try
        {
            await _connection.ExecuteAsync(
                new CommandDefinition(
                    departmentSql,
                    new
                    {
                        Id = department.Id,
                        Name = department.Name,
                        Slug = department.Slug,
                        Path = department.Path,
                        ParentId = department.ParentId,
                        CreatedAt = department.CreatedAt,
                        UpdatedAt = department.UpdatedAt
                    },
                    transaction: transaction,
                    cancellationToken: cancellationToken));

            foreach (var locationId in locationIds.Distinct())
            {
                await _connection.ExecuteAsync(
                    new CommandDefinition(
                        departmentLocationSql,
                        new
                        {
                            Id = Guid.NewGuid(),
                            DepartmentId = department.Id,
                            LocationId = locationId,
                            IsPrimaryLocation = false
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "Failed to save department {DepartmentId} with name {Name}", department.Id, department.Name);
            throw;
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<bool> UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        const string updateSql = """
        UPDATE departments
        SET name = @Name, updated_at = @UpdatedAt
        WHERE id = @Id
        """;

        var rows = await _connection.ExecuteAsync(
            new CommandDefinition(
                updateSql,
                new { department.Id, department.Name, department.UpdatedAt },
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        const string deleteSql = """
        DELETE FROM departments
        WHERE id = @Id
        """;

        var rows = await _connection.ExecuteAsync(
            new CommandDefinition(
                deleteSql,
                new { Id = id },
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, name, slug, path, parent_id AS ParentId, created_at AS CreatedAt, updated_at AS UpdatedAt
        FROM departments
        """;

        var departments = await _connection.QueryAsync<Department>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return departments.AsList();
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, name, slug, path, parent_id AS ParentId, created_at AS CreatedAt, updated_at AS UpdatedAt
        FROM departments
        WHERE id = @Id
        """;

        return await _connection.QuerySingleOrDefaultAsync<Department>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task UpdateDepartmentNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        const string updateNameSql = """
        UPDATE departments
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
            _logger.LogError(ex, "Failed to update department name to {Name}", name);
            throw;
        }
    }

    public async Task<bool> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT EXISTS(
            SELECT 1
            FROM department_locations
            WHERE department_id = @DepartmentId AND location_id = @LocationId
        )
        """;

        return await _connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { DepartmentId = departmentId, LocationId = locationId }, cancellationToken: cancellationToken));
    }

    public async Task AddLocationLinkAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        const string sql = """
        INSERT INTO department_locations (id, department_id, location_id, is_primary_location)
        VALUES (@Id, @DepartmentId, @LocationId, FALSE)
        """;

        await _connection.ExecuteAsync(
            new CommandDefinition(
                sql,
                new { Id = Guid.NewGuid(), DepartmentId = departmentId, LocationId = locationId },
                cancellationToken: cancellationToken));
    }

    public async Task<bool> RemoveLocationLinkAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        const string sql = """
        DELETE FROM department_locations
        WHERE department_id = @DepartmentId AND location_id = @LocationId
        """;

        var rows = await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { DepartmentId = departmentId, LocationId = locationId }, cancellationToken: cancellationToken));

        return rows > 0;
    }
}
