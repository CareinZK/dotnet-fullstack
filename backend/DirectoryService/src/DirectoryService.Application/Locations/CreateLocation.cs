using DirectoryService.Contracts;
using DirectoryService.Domain;
using FluentValidation;
using DirectoryService.Application.Exceptions;


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

    public async Task<Guid> ExecuteAsync(CreateLocationDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        if (await _locationRepository.NameExistsAsync(dto.Name, cancellationToken))
        {
            throw new LocationAlreadyExistsException($"Location with name '{dto.Name}' already exists.");
        }

        var location = new Location(Guid.NewGuid(), dto.Name, dto.Address);
        await _locationRepository.AddAsync(location, cancellationToken);

        return location.Id;
    }
}
