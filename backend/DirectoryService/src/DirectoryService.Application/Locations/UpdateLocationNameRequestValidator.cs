using DirectoryService.Contracts;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationNameRequestValidator : AbstractValidator<UpdateLocationNameRequest>
{
    public UpdateLocationNameRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Location id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Location name is required.")
            .MaximumLength(AppConstants.MaxNameLength)
            .WithMessage("Location name cannot exceed 200 characters.");
    }
}
