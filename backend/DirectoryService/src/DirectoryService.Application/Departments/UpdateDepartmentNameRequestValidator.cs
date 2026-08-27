using DirectoryService.Contracts;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public sealed class UpdateDepartmentNameRequestValidator : AbstractValidator<UpdateDepartmentNameRequest>
{
    public UpdateDepartmentNameRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Department id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(AppConstants.MaxNameLength)
            .WithMessage("Department name cannot exceed 200 characters.");
    }
}
