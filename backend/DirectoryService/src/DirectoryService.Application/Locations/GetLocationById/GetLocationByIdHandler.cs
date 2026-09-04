using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain;
using DirectoryService.Domain.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class GetLocationByIdHandler : IQueryHandler<GetLocationByIdQuery, LocationDto>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationByIdHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<Result<LocationDto, ErrorList>> Handle(GetLocationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var locationResult = await _locationRepository.GetByIdAsync(query.Id, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        return Map(locationResult.Value);
    }

    private static LocationDto Map(Location location) =>
        new(location.Id, location.Name, location.Address);
}
