using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationNameHandler
{
    private readonly ILocationRepository _repository;
    private readonly IValidator<UpdateLocationNameRequest> _validator;

    public UpdateLocationNameHandler(
        ILocationRepository locationRepository,
        IValidator<UpdateLocationNameRequest> validator)
    {
        _repository = locationRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> Handle(UpdateLocationNameRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var updateResult = await _repository.UpdateLocationNameAsync(request.Id, request.Name, cancellationToken);
        if (updateResult.IsFailure)
        {
            return updateResult.Error.ToErrorList();
        }

        return Result.Success<Guid, ErrorList>(request.Id);
    }
}