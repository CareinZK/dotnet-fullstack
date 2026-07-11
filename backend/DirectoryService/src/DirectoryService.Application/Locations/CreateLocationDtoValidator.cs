using DirectoryService.Contracts;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public sealed class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Location name is required.")
            .MaximumLength(AppConstants.Dto200)
            .WithMessage("Location name cannot exceed 200 characters.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Location address is required.")
            .MaximumLength(AppConstants.Dto500)
            .WithMessage("Location address cannot exceed 500 characters.");
    }
}
