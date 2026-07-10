namespace TemplateService.Contracts;

public sealed record PositionDto(Guid Id, string Name);

public sealed record CreatePositionDto(string Name);

public sealed record UpdatePositionDto(string Name);

public interface IPositionsService
{
    Task<PositionDto> CreateAsync(CreatePositionDto dto, CancellationToken cancellationToken);
    Task<IReadOnlyList<PositionDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PositionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdatePositionDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}