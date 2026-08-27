using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain;
using DirectoryService.Domain.Common;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public sealed class CreateLocation
{
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<CreateLocationDto> _validator;

    public CreateLocation(ILocationRepository locationRepository, IValidator<CreateLocationDto> validator)
    {
        _locationRepository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> ExecuteAsync(CreateLocationDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameExistsResult = await _locationRepository.NameExistsAsync(dto.Name, cancellationToken);
        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrorList();
        }

        if (nameExistsResult.Value)
        {
            return Errors.Location.AlreadyExists(dto.Name).ToErrorList();
        }

        var locationResult = Location.Create(Guid.NewGuid(), dto.Name, dto.Address);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        var addResult = await _locationRepository.AddAsync(locationResult.Value, cancellationToken);
        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrorList();
        }

        return Result.Success<Guid, ErrorList>(locationResult.Value.Id);
    }
}
