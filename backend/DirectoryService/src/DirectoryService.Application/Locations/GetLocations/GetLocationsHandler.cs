using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain;
using DirectoryService.Domain.Common;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class GetLocationsHandler : IQueryHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationsHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<Result<IReadOnlyList<LocationDto>, ErrorList>> Handle(GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var locationsResult = await _locationRepository.GetAllAsync(cancellationToken);
        if (locationsResult.IsFailure)
        {
            return locationsResult.Error.ToErrorList();
        }

        IReadOnlyList<LocationDto> dtos = locationsResult.Value.Select(Map).ToList();
        return Result.Success<IReadOnlyList<LocationDto>, ErrorList>(dtos);
    }

    private static LocationDto Map(Location location) =>
        new(location.Id, location.Name, location.Address);
}
