using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation;

[ApiController]
[Route("locations")]
[ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    public async Task<IResult> CreateLocationDto(
        [FromBody] CreateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var result = await _createLocation.ExecuteAsync(locationDto, cancellationToken);
        return result.ToCreatedEnvelopeResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(Envelope<IReadOnlyList<LocationDto>>), StatusCodes.Status200OK)]
    public async Task<IResult> GetLocations(
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.GetAllAsync(cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Envelope<LocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> GetLocationById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.GetByIdAsync(id, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpPatch("{id:guid}")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateLocation(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationDto locationDto,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.UpdateAsync(id, locationDto, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> DeleteLocation(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.DeleteAsync(id, cancellationToken);
        return result.ToEnvelopeResult();
    }

    [HttpPatch("{id:guid}/name")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    public async Task<IResult> UpdateLocationName(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationNameRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            ErrorList error = Errors.General.ValueIsInvalid(nameof(request.Id), "The route id and request id do not match.");
            return error.ToEnvelopeResult();
        }

        var result = await _updateLocationNameHandler.Handle(request, cancellationToken);
        return result.ToEnvelopeResult();
    }
}
