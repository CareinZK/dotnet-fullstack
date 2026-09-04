using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class UpdateDepartmentNameHandler : ICommandHandler<UpdateDepartmentNameCommand, Guid>
{
    private readonly IDepartmentRepository _repository;
    private readonly IValidator<UpdateDepartmentNameCommand> _validator;

    public UpdateDepartmentNameHandler(
        IDepartmentRepository departmentRepository,
        IValidator<UpdateDepartmentNameCommand> validator)
    {
        _repository = departmentRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> Handle(UpdateDepartmentNameCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var updateResult = await _repository.UpdateDepartmentNameAsync(command.Id, command.Name, cancellationToken);
        if (updateResult.IsFailure)
        {
            return updateResult.Error.ToErrorList();
        }

        return Result.Success<Guid, ErrorList>(command.Id);
    }

    // ReSharper disable once UnusedMember.Global
    public Task<Result<Guid, ErrorList>> Handle(UpdateDepartmentNameRequest request, CancellationToken cancellationToken = default) =>
        Handle(new UpdateDepartmentNameCommand(request.Id, request.Name), cancellationToken);
}
