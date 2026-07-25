using DirectoryService.Contracts;

namespace DirectoryService.Presentation;

// Keeps the API contract available while the real application services are not implemented yet.
// These services deliberately do not provide persistence or in-memory behavior.
public sealed class StubLocationsService : ILocationsService
{
    public Task<LocationDto> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<LocationDto>();

    public Task<IReadOnlyList<LocationDto>> GetAllAsync(CancellationToken cancellationToken) => StubServiceResult.NotImplemented<IReadOnlyList<LocationDto>>();

    public Task<LocationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<LocationDto?>();

    public Task<bool> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<bool>();

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<bool>();

}

public sealed class StubDepartmentsService : IDepartmentsService
{
    public Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<DepartmentDto>();

    public Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken) => StubServiceResult.NotImplemented<IReadOnlyList<DepartmentDto>>();

    public Task<DepartmentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<DepartmentDto?>();

    public Task<bool> UpdateAsync(Guid id, UpdateDepartmentDto dto, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<bool>();

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<bool>();

    public Task LinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken) => StubServiceResult.NotImplemented();

    public Task UnlinkLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken) => StubServiceResult.NotImplemented();
}

public sealed class StubPositionsService : IPositionsService
{
    public Task<PositionDto> CreateAsync(CreatePositionDto dto, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<PositionDto>();

    public Task<IReadOnlyList<PositionDto>> GetAllAsync(CancellationToken cancellationToken) => StubServiceResult.NotImplemented<IReadOnlyList<PositionDto>>();

    public Task<PositionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<PositionDto?>();

    public Task<bool> UpdateAsync(Guid id, UpdatePositionDto dto, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<bool>();

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken) => StubServiceResult.NotImplemented<bool>();
}

file static class StubServiceResult
{
    public static Task<T> NotImplemented<T>() =>
        Task.FromException<T>(new NotSupportedException("This API operation has not been implemented."));

    public static Task NotImplemented() =>
        Task.FromException(new NotSupportedException("This API operation has not been implemented."));
}
