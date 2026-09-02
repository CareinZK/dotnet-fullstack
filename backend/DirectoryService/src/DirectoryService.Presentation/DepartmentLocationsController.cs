using DirectoryService.Contracts;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("departments/{departmentId:guid}/locations")]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
public sealed class DepartmentLocationsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentLocationsController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }

    [HttpPost("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    public async Task<IResult> LinkLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.LinkLocationAsync(departmentId, locationId, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpDelete("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> UnlinkLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.UnlinkLocationAsync(departmentId, locationId, cancellationToken);
        return result.ToEnvelopeResult();
    }
}
