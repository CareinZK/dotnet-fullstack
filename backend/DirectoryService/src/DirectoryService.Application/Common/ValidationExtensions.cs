using DirectoryService.Domain.Common;
using FluentValidation.Results;

namespace DirectoryService.Application.Common;

public static class ValidationExtensions
{
    public static ErrorList ToErrorList(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors.Select(failure =>
            Error.Validation(
                string.IsNullOrWhiteSpace(failure.ErrorCode) ? "validation.error" : failure.ErrorCode,
                failure.ErrorMessage,
                failure.PropertyName)).ToList();

        return new ErrorList(errors);
    }
}
