using Microsoft.AspNetCore.Mvc;
using DirectoryService.Contracts;

namespace DirectoryService.Presentation;

[ApiController]
[Route("locations")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;

    public LocationsController(ILocationsService locationsService)
    {
        _locationsService = locationsService;
    }

    [HttpPost]
    public async Task<ActionResult<LocationDto>> CreateLocationDto(
        [FromBody] CreateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var createdLocation = await _locationsService.CreateAsync(locationDto, cancellationToken);

        return CreatedAtAction(
            nameof(GetLocationById),
            new { id = createdLocation.Id },
            createdLocation);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocationDto>>> GetLocations(
        CancellationToken cancellationToken)
    {
        var locations = await _locationsService.GetAllAsync(cancellationToken);
        return Ok(locations);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LocationDto>> GetLocationById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var location = await _locationsService.GetByIdAsync(id, cancellationToken);

        if (location is null)
        {
            return NotFound();
        }

        return Ok(location);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLocation(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var updated = await _locationsService.UpdateAsync(id, locationDto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLocation(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await _locationsService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}