using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;

namespace DirectoryService.Contracts;

public sealed record PositionDto(Guid Id, string Name);

public sealed record CreatePositionDto(string Name);

public sealed record UpdatePositionDto(string Name);

public interface IPositionsService
{
    Task<Result<PositionDto, ErrorList>> CreateAsync(CreatePositionDto dto, CancellationToken cancellationToken);
    Task<Result<IReadOnlyList<PositionDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<PositionDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdatePositionDto dto, CancellationToken cancellationToken);
    Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken);
}