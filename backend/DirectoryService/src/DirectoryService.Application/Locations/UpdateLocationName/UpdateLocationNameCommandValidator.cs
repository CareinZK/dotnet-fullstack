using DirectoryService.Contracts;
using FluentValidation;

// ReSharper disable once CheckNamespace
namespace DirectoryService.Application.Locations;

public sealed class UpdateLocationNameCommandValidator : AbstractValidator<UpdateLocationNameCommand>
{
    public UpdateLocationNameCommandValidator()
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

// ReSharper disable once UnusedType.Global
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
