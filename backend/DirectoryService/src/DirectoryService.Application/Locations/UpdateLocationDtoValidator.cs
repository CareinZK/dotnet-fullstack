using DirectoryService.Contracts;
using FluentValidation;

namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationDtoValidator : AbstractValidator<UpdateLocationDto>
{
    public UpdateLocationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Location name is required.")
            .MaximumLength(AppConstants.MaxNameLength)
            .WithMessage("Location name cannot exceed 200 characters.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Location address is required.")
            .MaximumLength(AppConstants.MaxAddressLength)
            .WithMessage("Location address cannot exceed 500 characters.");
    }
}
