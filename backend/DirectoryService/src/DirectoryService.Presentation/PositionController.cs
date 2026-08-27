using DirectoryService.Contracts;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("positions")]
#pragma warning disable CA1515 // Consider making public types internal
public sealed class PositionsController : ControllerBase
#pragma warning restore CA1515 // Consider making public types internal
{
    private readonly IPositionsService _positionsService;

    public PositionsController(IPositionsService positionsService)
    {
        _positionsService = positionsService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePosition(
        [FromBody] CreatePositionDto positionDto,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.CreateAsync(positionDto, cancellationToken);

        return result.ToCreatedAtActionResult(
            nameof(GetPositionById),
            new { id = result.IsSuccess ? result.Value.Id : Guid.Empty });
    }

    [HttpGet]
    public async Task<IActionResult> GetPositions(
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.GetAllAsync(cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPositionById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePosition(
        [FromRoute] Guid id,
        [FromBody] UpdatePositionDto positionDto,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.UpdateAsync(id, positionDto, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePosition(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.DeleteAsync(id, cancellationToken);
        return result.ToNoContentActionResult();
    }
}