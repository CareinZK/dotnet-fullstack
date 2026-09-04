using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain;
using DirectoryService.Domain.Common;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class CreateLocationHandler : ICommandHandler<CreateLocationCommand, Guid>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<CreateLocationCommand> _validator;

    public CreateLocationHandler(
        ILocationRepository locationRepository,
        IValidator<CreateLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> Handle(CreateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var nameExistsResult = await _locationRepository.NameExistsAsync(command.Name, cancellationToken);
        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrorList();
        }

        if (nameExistsResult.Value)
        {
            return Errors.Location.AlreadyExists(command.Name).ToErrorList();
        }

        var locationResult = Location.Create(Guid.NewGuid(), command.Name, command.Address);
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

    public Task<Result<Guid, ErrorList>> ExecuteAsync(CreateLocationDto dto, CancellationToken cancellationToken = default) =>
        Handle(new CreateLocationCommand(dto.Name, dto.Address), cancellationToken);
}

public sealed class CreateLocation
{
    private readonly CreateLocationHandler _handler;

    public CreateLocation(ILocationRepository locationRepository, IValidator<CreateLocationDto> validator)
    {
        _ = validator;
        _handler = new CreateLocationHandler(locationRepository, new CreateLocationCommandValidator());
    }

    public Task<Result<Guid, ErrorList>> ExecuteAsync(CreateLocationDto dto, CancellationToken cancellationToken = default) =>
        _handler.ExecuteAsync(dto, cancellationToken);
}
