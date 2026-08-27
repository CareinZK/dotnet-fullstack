using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain;
using DirectoryService.Domain.Common;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public sealed class LocationsService : ILocationsService
{
    private readonly ILocationRepository _locationRepository;
    private readonly CreateLocation _createLocation;
    private readonly IValidator<UpdateLocationDto> _updateValidator;

    public LocationsService(
        ILocationRepository locationRepository,
        CreateLocation createLocation,
        IValidator<UpdateLocationDto> updateValidator)
    {
        _locationRepository = locationRepository;
        _createLocation = createLocation;
        _updateValidator = updateValidator;
    }

    public async Task<Result<LocationDto, ErrorList>> CreateAsync(CreateLocationDto dto, CancellationToken cancellationToken)
    {
        var result = await _createLocation.ExecuteAsync(dto, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error;
        }

        var locationResult = await _locationRepository.GetByIdAsync(result.Value, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        return Map(locationResult.Value);
    }

    public async Task<Result<IReadOnlyList<LocationDto>, ErrorList>> GetAllAsync(CancellationToken cancellationToken)
    {
        var locationsResult = await _locationRepository.GetAllAsync(cancellationToken);
        if (locationsResult.IsFailure)
        {
            return locationsResult.Error.ToErrorList();
        }

        IReadOnlyList<LocationDto> dtos = locationsResult.Value.Select(Map).ToList();
        return Result.Success<IReadOnlyList<LocationDto>, ErrorList>(dtos);
    }

    public async Task<Result<LocationDto, ErrorList>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var locationResult = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        return Map(locationResult.Value);
    }

    public async Task<UnitResult<ErrorList>> UpdateAsync(Guid id, UpdateLocationDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var locationResult = await _locationRepository.GetByIdAsync(id, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        var location = locationResult.Value;
        var updateDetailsResult = location.UpdateDetails(dto.Name, dto.Address);
        if (updateDetailsResult.IsFailure)
        {
            return updateDetailsResult.Error.ToErrorList();
        }

        var updateRepoResult = await _locationRepository.UpdateAsync(location, cancellationToken);
        if (updateRepoResult.IsFailure)
        {
            return updateRepoResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    public async Task<UnitResult<ErrorList>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleteResult = await _locationRepository.DeleteAsync(id, cancellationToken);
        if (deleteResult.IsFailure)
        {
            return deleteResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }

    private static LocationDto Map(Location location) =>
        new(location.Id, location.Name, location.Address);
}
