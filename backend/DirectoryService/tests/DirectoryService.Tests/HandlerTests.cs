using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain;
using DirectoryService.Domain.Common;
using DirectoryService.Domain.Departments;
using Xunit;

namespace DirectoryService.Tests;

public sealed class HandlerTests
{
    private sealed class FakeLocationRepository : ILocationRepository
    {
        public List<Location> Locations { get; } = [];
        public bool ShouldFailWithDbError { get; set; }

        public Task<Result<bool, Error>> NameExistsAsync(string name, CancellationToken cancellationToken)
        {
            if (ShouldFailWithDbError)
            {
                return Task.FromResult(Result.Failure<bool, Error>(Error.Failure("database.error", "DB failure")));
            }

            return Task.FromResult(Result.Success<bool, Error>(Locations.Any(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase))));
        }

        public Task<UnitResult<Error>> AddAsync(Location location, CancellationToken cancellationToken)
        {
            if (ShouldFailWithDbError)
            {
                return Task.FromResult(UnitResult.Failure<Error>(Error.Failure("database.error", "DB failure")));
            }

            Locations.Add(location);
            return Task.FromResult(UnitResult.Success<Error>());
        }

        public Task<Result<IReadOnlyList<Location>, Error>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success<IReadOnlyList<Location>, Error>(Locations));

        public Task<Result<Location, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var loc = Locations.FirstOrDefault(l => l.Id == id);
            return Task.FromResult(loc is not null
                ? Result.Success<Location, Error>(loc)
                : Result.Failure<Location, Error>(Errors.Location.NotFound(id)));
        }

        public Task<UnitResult<Error>> UpdateAsync(Location location, CancellationToken cancellationToken) =>
            Task.FromResult(UnitResult.Success<Error>());

        public Task<UnitResult<Error>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Locations.RemoveAll(l => l.Id == id);
            return Task.FromResult(UnitResult.Success<Error>());
        }

        public Task<UnitResult<Error>> UpdateLocationNameAsync(Guid id, string name, CancellationToken cancellationToken) =>
            Task.FromResult(UnitResult.Success<Error>());
    }

    private sealed class FakeDepartmentRepository : IDepartmentRepository
    {
        public List<Department> Departments { get; } = [];
        public HashSet<(Guid, Guid)> Links { get; } = [];

        public Task<Result<bool, Error>> NameExistsAsync(string name, CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success<bool, Error>(Departments.Any(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase))));

        public Task<UnitResult<Error>> AddAsync(Department department, IReadOnlyCollection<Guid> locationIds, CancellationToken cancellationToken)
        {
            Departments.Add(department);
            foreach (var locId in locationIds)
            {
                Links.Add((department.Id, locId));
            }
            return Task.FromResult(UnitResult.Success<Error>());
        }

        public Task<Result<IReadOnlyList<Department>, Error>> GetAllAsync(CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success<IReadOnlyList<Department>, Error>(Departments));

        public Task<Result<Department, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var dept = Departments.FirstOrDefault(d => d.Id == id);
            return Task.FromResult(dept is not null
                ? Result.Success<Department, Error>(dept)
                : Result.Failure<Department, Error>(Errors.Department.NotFound(id)));
        }

        public Task<UnitResult<Error>> UpdateAsync(Department department, CancellationToken cancellationToken) =>
            Task.FromResult(UnitResult.Success<Error>());

        public Task<UnitResult<Error>> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Departments.RemoveAll(d => d.Id == id);
            return Task.FromResult(UnitResult.Success<Error>());
        }

        public Task<UnitResult<Error>> UpdateDepartmentNameAsync(Guid id, string name, CancellationToken cancellationToken) =>
            Task.FromResult(UnitResult.Success<Error>());

        public Task<Result<bool, Error>> LocationLinkExistsAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken) =>
            Task.FromResult(Result.Success<bool, Error>(Links.Contains((departmentId, locationId))));

        public Task<UnitResult<Error>> AddLocationLinkAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            Links.Add((departmentId, locationId));
            return Task.FromResult(UnitResult.Success<Error>());
        }

        public Task<UnitResult<Error>> RemoveLocationLinkAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
        {
            var removed = Links.Remove((departmentId, locationId));
            return Task.FromResult(removed
                ? UnitResult.Success<Error>()
                : UnitResult.Failure<Error>(Errors.Department.LocationNotLinked(departmentId, locationId)));
        }
    }

    [Fact]
    public async Task CreateLocation_WithDuplicateName_ReturnsConflictError()
    {
        var repo = new FakeLocationRepository();
        repo.Locations.Add(Location.Create(Guid.NewGuid(), "Existing Office", "Address").Value);

        var handler = new CreateLocation(repo, new CreateLocationDtoValidator());
        var result = await handler.ExecuteAsync(new CreateLocationDto("Existing Office", "New Address"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error[0].Type);
        Assert.Equal("location.already.exists", result.Error[0].Code);
    }

    [Fact]
    public async Task CreateLocation_WhenDatabaseFails_ReturnsDatabaseError()
    {
        var repo = new FakeLocationRepository { ShouldFailWithDbError = true };

        var handler = new CreateLocation(repo, new CreateLocationDtoValidator());
        var result = await handler.ExecuteAsync(new CreateLocationDto("New Office", "Address"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Failure, result.Error[0].Type);
        Assert.Equal("database.error", result.Error[0].Code);
    }

    [Fact]
    public async Task CreateDepartment_WithNonExistentLocation_ReturnsNotFoundError()
    {
        var deptRepo = new FakeDepartmentRepository();
        var locRepo = new FakeLocationRepository();
        var nonExistentLocId = Guid.NewGuid();

        var handler = new CreateDepartment(deptRepo, locRepo, new CreateDepartmentDtoValidator());
        var result = await handler.ExecuteAsync(
            new CreateDepartmentDto("Engineering", "engineering", null, [nonExistentLocId]),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error[0].Type);
        Assert.Equal("location.not.found", result.Error[0].Code);
    }

    [Fact]
    public async Task CreateDepartment_WithNonExistentParent_ReturnsNotFoundError()
    {
        var deptRepo = new FakeDepartmentRepository();
        var locRepo = new FakeLocationRepository();
        var nonExistentParentId = Guid.NewGuid();

        var handler = new CreateDepartment(deptRepo, locRepo, new CreateDepartmentDtoValidator());
        var result = await handler.ExecuteAsync(
            new CreateDepartmentDto("Engineering", "engineering", nonExistentParentId, null),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error[0].Type);
        Assert.Equal("department.parent.not.found", result.Error[0].Code);
    }

    [Fact]
    public async Task DepartmentsService_LinkLocation_WhenAlreadyLinked_ReturnsConflict()
    {
        var deptRepo = new FakeDepartmentRepository();
        var locRepo = new FakeLocationRepository();

        var dept = Department.Create(Guid.NewGuid(), "Finance", "finance", null).Value;
        var loc = Location.Create(Guid.NewGuid(), "HQ", "Main St").Value;
        deptRepo.Departments.Add(dept);
        locRepo.Locations.Add(loc);
        deptRepo.Links.Add((dept.Id, loc.Id));

        var service = new DepartmentsService(deptRepo, locRepo, new CreateDepartmentDtoValidator(), new UpdateDepartmentDtoValidator());
        var result = await service.LinkLocationAsync(dept.Id, loc.Id, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error[0].Type);
        Assert.Equal("department.location.already.linked", result.Error[0].Code);
    }

    [Fact]
    public async Task DepartmentsService_UnlinkLocation_WhenNotLinked_ReturnsNotFound()
    {
        var deptRepo = new FakeDepartmentRepository();
        var locRepo = new FakeLocationRepository();

        var dept = Department.Create(Guid.NewGuid(), "Finance", "finance", null).Value;
        var loc = Location.Create(Guid.NewGuid(), "HQ", "Main St").Value;
        deptRepo.Departments.Add(dept);
        locRepo.Locations.Add(loc);

        var service = new DepartmentsService(deptRepo, locRepo, new CreateDepartmentDtoValidator(), new UpdateDepartmentDtoValidator());
        var result = await service.UnlinkLocationAsync(dept.Id, loc.Id, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error[0].Type);
        Assert.Equal("department.location.not.linked", result.Error[0].Code);
    }
}
