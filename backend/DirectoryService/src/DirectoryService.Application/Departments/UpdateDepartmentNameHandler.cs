using CSharpFunctionalExtensions;
using DirectoryService.Application.Common;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public sealed class UpdateDepartmentNameHandler
{
    private readonly IDepartmentRepository _repository;
    private readonly IValidator<UpdateDepartmentNameRequest> _validator;

    public UpdateDepartmentNameHandler(
        IDepartmentRepository departmentRepository,
        IValidator<UpdateDepartmentNameRequest> validator)
    {
        _repository = departmentRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, ErrorList>> Handle(UpdateDepartmentNameRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrorList();
        }

        var updateResult = await _repository.UpdateDepartmentNameAsync(request.Id, request.Name, cancellationToken);
        if (updateResult.IsFailure)
        {
            return updateResult.Error.ToErrorList();
        }

        return Result.Success<Guid, ErrorList>(request.Id);
    }
}