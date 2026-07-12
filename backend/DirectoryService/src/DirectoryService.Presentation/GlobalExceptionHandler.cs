using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DirectoryService.Application.Exceptions;

namespace DirectoryService.Presentation;

#pragma warning disable CA1515 // Consider making public types internal
public sealed class GlobalExceptionHandler : IExceptionHandler
#pragma warning restore CA1515 // Consider making public types internal
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case ValidationException validationException:
            {
                var errors = validationException.Errors
                    .GroupBy(x => x.PropertyName, StringComparer.Ordinal)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray(),
                        StringComparer.Ordinal);

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed."
                };

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            case LocationAlreadyExistsException ex:
            {
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

                await httpContext.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Conflict",
                        Detail = ex.Message
                    },
                    cancellationToken);

                return true;
            }

            default:
                return false;
        }
    }
}