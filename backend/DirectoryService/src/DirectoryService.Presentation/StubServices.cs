using CSharpFunctionalExtensions;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;

namespace DirectoryService.Presentation;

// =========================================================================================================
// NOTE ON STUB SERVICES:
// These stub classes were initially created as temporary placeholders in early versions of the project
// before the real application services and database repositories were implemented.
//
// Current status:
// - StubPositionsService: TEMPORARILY USED (until PositionsService is implemented in a future module)
// =========================================================================================================

/// <summary>
/// Temporary stub: still registered in Program.cs until PositionsService is implemented.
/// </summary>
public sealed class StubPositionsService : IPositionsService
{
    public Task<Result<PositionDto, ErrorList>> CreateAsync(CreatePositionDto dto, CancellationToken cancellationToken) =>
        StubServiceResult.NotImplemented<PositionDto>();

    public Task<Result<IReadOnlyList<PositionDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken) =>
        StubServiceResult.NotImplemented<IReadOnlyList<PositionDto>>();

    public Task<Result<PositionDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        StubServiceResult.NotImplemented<PositionDto>();

    public Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdatePositionDto dto, CancellationToken cancellationToken) =>
        StubServiceResult.NotImplemented();

    public Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
        StubServiceResult.NotImplemented();
}

file static class StubServiceResult
{
    public static Task<Result<T, ErrorList>> NotImplemented<T>() =>
        Task.FromResult(Result.Failure<T, ErrorList>(Errors.General.Failure("This API operation has not been implemented.")));

    public static Task<UnitResult<ErrorList>> NotImplemented() =>
        Task.FromResult(UnitResult.Failure<ErrorList>(Errors.General.Failure("This API operation has not been implemented.")));
}
