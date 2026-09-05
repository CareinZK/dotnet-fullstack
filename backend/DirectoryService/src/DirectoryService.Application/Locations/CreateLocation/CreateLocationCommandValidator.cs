using DirectoryService.Contracts;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
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

public sealed class CreateLocationDtoValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationDtoValidator()
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
