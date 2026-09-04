using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationNameHandler : ICommandHandler<UpdateLocationNameCommand, Guid>
{
    private readonly ILocationRepository _repository;
    private readonly IValidator<UpdateLocationNameCommand> _validator;

    public UpdateLocationNameHandler(
        ILocationRepository locationRepository,
        IValidator<UpdateLocationNameCommand> validator)
    {
        _repository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> Handle(UpdateLocationNameCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var updateResult = await _repository.UpdateLocationNameAsync(command.Id, command.Name, cancellationToken);
        if (updateResult.IsFailure)
        {
            return updateResult.Error.ToErrorList();
        }

        return Result.Success<Guid, ErrorList>(command.Id);
    }

    // ReSharper disable once UnusedMember.Global
    public Task<Result<Guid, ErrorList>> Handle(UpdateLocationNameRequest request, CancellationToken cancellationToken = default) =>
        Handle(new UpdateLocationNameCommand(request.Id, request.Name), cancellationToken);
}
