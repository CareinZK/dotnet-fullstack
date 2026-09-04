using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Domain.Common;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationHandler : ICommandHandler<UpdateLocationCommand>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IValidator<UpdateLocationCommand> _validator;

    public UpdateLocationHandler(
        ILocationRepository locationRepository,
        IValidator<UpdateLocationCommand> validator)
    {
        _locationRepository = locationRepository;
        _validator = validator;
    }

    public async Task<UnitResult<ErrorList>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var locationResult = await _locationRepository.GetByIdAsync(command.Id, cancellationToken);
        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrorList();
        }

        var location = locationResult.Value;
        var updateDetailsResult = location.UpdateDetails(command.Name, command.Address);
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
}
