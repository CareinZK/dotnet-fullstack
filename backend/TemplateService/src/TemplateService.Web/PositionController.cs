using Microsoft.AspNetCore.Mvc;
using TemplateService.Contracts;

namespace TemplateService.Web;

[ApiController]
[Route("locations")]
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
    public async Task<ActionResult<PositionDto>> CreatePosition(
        [FromBody] CreatePositionDto positionDto,
        CancellationToken cancellationToken)
    {
        var createdPosition = await _positionsService.CreateAsync(positionDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetPositionById),
            new { id = createdPosition.Id },
            createdPosition);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PositionDto>>> GetPositions(
        CancellationToken cancellationToken)
    {
        var positions = await _positionsService.GetAllAsync(cancellationToken);
        return Ok(positions);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PositionDto>> GetPositionById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var position = await _positionsService.GetByIdAsync(id, cancellationToken);

        if (position is null)
        {
            return NotFound();
        }

        return Ok(position);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePosition(
        [FromRoute] Guid id,
        [FromBody] UpdatePositionDto positionDto,
        CancellationToken cancellationToken)
    {
        var updated = await _positionsService.UpdateAsync(id, positionDto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePosition(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _positionsService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}