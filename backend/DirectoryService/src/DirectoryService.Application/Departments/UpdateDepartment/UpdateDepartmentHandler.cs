using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Domain.Common;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Departments;

public sealed class UpdateDepartmentHandler : ICommandHandler<UpdateDepartmentCommand>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IValidator<UpdateDepartmentCommand> _validator;

    public UpdateDepartmentHandler(
        IDepartmentRepository departmentRepository,
        IValidator<UpdateDepartmentCommand> validator)
    {
        _departmentRepository = departmentRepository;
        _validator = validator;
    }

    public async Task<UnitResult<ErrorList>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var departmentResult = await _departmentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrorList();
        }

        var department = departmentResult.Value;
        var changeNameResult = department.ChangeName(command.Name);
        if (changeNameResult.IsFailure)
        {
            return changeNameResult.Error.ToErrorList();
        }

        var updateRepoResult = await _departmentRepository.UpdateAsync(department, cancellationToken);
        if (updateRepoResult.IsFailure)
        {
            return updateRepoResult.Error.ToErrorList();
        }

        return UnitResult.Success<ErrorList>();
    }
}
