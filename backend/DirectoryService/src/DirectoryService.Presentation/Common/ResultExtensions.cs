using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace DirectoryService.Presentation.Common;

public static class ResultExtensions
{
    public static EnvelopeResult<T> ToEnvelopeResult<T>(
        this Result<T, ErrorList> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return new EnvelopeResult<T>(Envelope<T>.Ok(result.Value), successStatusCode);
        }

        var statusCode = GetStatusCode(result.Error);
        return new EnvelopeResult<T>(Envelope<T>.Error(result.Error), statusCode);
    }

    public static EnvelopeResult<T> ToEnvelopeResult<T>(
        this Result<T, Error> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        return result.ToErrorList().ToEnvelopeResult(successStatusCode);
    }

    public static EnvelopeResult ToEnvelopeResult(
        this UnitResult<ErrorList> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        if (result.IsSuccess)
        {
            return new EnvelopeResult(Envelope.Ok(), successStatusCode);
        }

        var statusCode = GetStatusCode(result.Error);
        return new EnvelopeResult(Envelope.Error(result.Error), statusCode);
    }

    public static EnvelopeResult ToEnvelopeResult(
        this UnitResult<Error> result,
        int successStatusCode = StatusCodes.Status200OK)
    {
        return result.ToErrorList().ToEnvelopeResult(successStatusCode);
    }

    public static EnvelopeResult ToEnvelopeResult(this ErrorList errors)
    {
        var statusCode = GetStatusCode(errors);
        return new EnvelopeResult(Envelope.Error(errors), statusCode);
    }

    public static EnvelopeResult ToEnvelopeResult(this Error error)
    {
        var errors = error.ToErrorList();
        var statusCode = GetStatusCode(errors);
        return new EnvelopeResult(Envelope.Error(errors), statusCode);
    }

    public static EnvelopeResult<T> ToCreatedEnvelopeResult<T>(
        this Result<T, ErrorList> result)
    {
        return result.ToEnvelopeResult(StatusCodes.Status201Created);
    }

    public static EnvelopeResult<T> ToCreatedEnvelopeResult<T>(
        this Result<T, Error> result)
    {
        return result.ToEnvelopeResult(StatusCodes.Status201Created);
    }

    public static int GetStatusCode(ErrorList errors)
    {
        if (errors.Count == 0)
        {
            return StatusCodes.Status500InternalServerError;
        }

        var primaryError = errors[0];
        return primaryError.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
