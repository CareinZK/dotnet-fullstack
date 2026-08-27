using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("locations")]
public sealed class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;
    private readonly CreateLocation _createLocation;
    private readonly UpdateLocationNameHandler _updateLocationNameHandler;

    public LocationsController(
        ILocationsService locationsService,
        CreateLocation createLocation,
        UpdateLocationNameHandler updateLocationNameHandler)
    {
        _locationsService = locationsService;
        _createLocation = createLocation;
        _updateLocationNameHandler = updateLocationNameHandler;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocationDto(
        [FromBody] CreateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var result = await _createLocation.ExecuteAsync(locationDto, cancellationToken);

        return result.ToCreatedAtActionResult(
            nameof(GetLocationById),
            new { id = result.IsSuccess ? result.Value : Guid.Empty });
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations(
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.GetAllAsync(cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLocationById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("{id:guid}")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLocation(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.UpdateAsync(id, locationDto, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLocation(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.DeleteAsync(id, cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpPatch("{id:guid}/name")]
    public async Task<IActionResult> UpdateLocationName(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationNameRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            ErrorList error = Errors.General.ValueIsInvalid(nameof(request.Id), "The route id and request id do not match.");
            return error.ToActionResult();
        }

        var result = await _updateLocationNameHandler.Handle(request, cancellationToken);
        return result.ToActionResult();
    }
}
