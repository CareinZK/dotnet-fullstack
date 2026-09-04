using DirectoryService.Contracts;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("positions")]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(Envelope<PositionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    public async Task<IResult> CreatePosition(
        [FromBody] CreatePositionDto positionDto,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.CreateAsync(positionDto, cancellationToken);
        return result.ToCreatedEnvelopeResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(Envelope<IReadOnlyList<PositionDto>>), StatusCodes.Status200OK)]
    public async Task<IResult> GetPositions(
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.GetAllAsync(cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Envelope<PositionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> GetPositionById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.GetByIdAsync(id, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdatePosition(
        [FromRoute] Guid id,
        [FromBody] UpdatePositionDto positionDto,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.UpdateAsync(id, positionDto, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> DeletePosition(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _positionsService.DeleteAsync(id, cancellationToken);
        return result.ToEnvelopeResult();
    }
}