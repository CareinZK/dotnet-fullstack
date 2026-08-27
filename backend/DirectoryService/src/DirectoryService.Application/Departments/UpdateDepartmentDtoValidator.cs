using DirectoryService.Contracts;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public sealed class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(AppConstants.MaxNameLength)
            .WithMessage("Department name cannot exceed 200 characters.");
    }
}
