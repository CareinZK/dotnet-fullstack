using DirectoryService.Contracts;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("departments/{departmentId:guid}/locations")]
public sealed class DepartmentLocationsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentLocationsController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }

    [HttpPost("{locationId:guid}")]
    public async Task<IActionResult> LinkLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.LinkLocationAsync(departmentId, locationId, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpDelete("{locationId:guid}")]
    public async Task<IActionResult> UnlinkLocation(
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var result = await _departmentsService.UnlinkLocationAsync(departmentId, locationId, cancellationToken);
        return result.ToNoContentActionResult();
    }
}
