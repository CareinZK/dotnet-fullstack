using DirectoryService.Contracts;
using FluentValidation;

namespace DirectoryService.Application.Departments;

public sealed class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(AppConstants.MaxNameLength)
            .WithMessage("Department name cannot exceed 200 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Department slug is required.")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Department slug must be a non-empty URL-safe string (lowercase letters, digits, hyphens).");
    }
}
