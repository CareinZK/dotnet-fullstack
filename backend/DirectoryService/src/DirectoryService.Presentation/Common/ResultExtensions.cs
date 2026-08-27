using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presentation.Common;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T, ErrorList> result)
    {
        return result.IsSuccess
            ? new OkObjectResult(result.Value)
            : result.Error.ToActionResult();
    }
    
    public static IActionResult ToCreatedAtActionResult<T>(
        this Result<T, ErrorList> result,
        string actionName,
        object routeValues)
    {
        return result.IsSuccess
            ? new CreatedAtActionResult(actionName, null, routeValues, result.Value)
            : result.Error.ToActionResult();
    }

    public static IActionResult ToNoContentActionResult(this UnitResult<ErrorList> result)
    {
        return result.IsSuccess
            ? new NoContentResult()
            : result.Error.ToActionResult();
    }

    public static IActionResult ToActionResult(this ErrorList errors)
    {
        if (errors.Count == 0)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        var primaryError = errors[0];
        var statusCode = primaryError.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        object responseBody = (errors.Count > 1 || primaryError.Type == ErrorType.Validation)
            ? (object)errors
            : (object)primaryError;

        return new ObjectResult(responseBody)
        {
            StatusCode = statusCode
        };
    }
}
