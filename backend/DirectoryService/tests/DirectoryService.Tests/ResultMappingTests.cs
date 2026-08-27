using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DirectoryService.Tests;

public sealed class ResultMappingTests
{
    [Fact]
    public void ToActionResult_Success_ReturnsOk200()
    {
        var result = Result.Success<string, ErrorList>("test-data");
        var actionResult = result.ToActionResult();

        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal("test-data", okResult.Value);
    }

    [Fact]
    public void ToCreatedAtActionResult_Success_ReturnsCreated201()
    {
        var id = Guid.NewGuid();
        var result = Result.Success<Guid, ErrorList>(id);
        var actionResult = result.ToCreatedAtActionResult("GetLocationById", new { id });

        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetLocationById", createdResult.ActionName);
        Assert.Equal(id, createdResult.Value);
    }

    [Fact]
    public void ToNoContentActionResult_Success_ReturnsNoContent204()
    {
        var result = UnitResult.Success<ErrorList>();
        var actionResult = result.ToNoContentActionResult();

        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact]
    public void ToActionResult_ValidationError_ReturnsBadRequest400WithErrors()
    {
        var errors = new ErrorList([
            Error.Validation("validation.name", "Name required", "Name"),
            Error.Validation("validation.address", "Address required", "Address")
        ]);
        var result = Result.Failure<string, ErrorList>(errors);

        var actionResult = result.ToActionResult();
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

        var responseList = Assert.IsAssignableFrom<IReadOnlyList<Error>>(objectResult.Value);
        Assert.Equal(2, responseList.Count);
    }

    [Fact]
    public void ToActionResult_NotFoundError_ReturnsNotFound404()
    {
        var error = Errors.Department.NotFound(Guid.NewGuid());
        var result = Result.Failure<string, ErrorList>(error.ToErrorList());

        var actionResult = result.ToActionResult();
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

        var responseError = Assert.IsType<Error>(objectResult.Value);
        Assert.Equal("department.not.found", responseError.Code);
    }

    [Fact]
    public void ToActionResult_ConflictError_ReturnsConflict409()
    {
        var error = Errors.Location.AlreadyExists("Main");
        var result = Result.Failure<string, ErrorList>(error.ToErrorList());

        var actionResult = result.ToActionResult();
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status409Conflict, objectResult.StatusCode);

        var responseError = Assert.IsType<Error>(objectResult.Value);
        Assert.Equal("location.already.exists", responseError.Code);
    }

    [Fact]
    public void ToActionResult_FailureError_ReturnsInternalServerError500()
    {
        var error = Error.Failure("database.error", "Database connection lost.");
        var result = Result.Failure<string, ErrorList>(error.ToErrorList());

        var actionResult = result.ToActionResult();
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

        var responseError = Assert.IsType<Error>(objectResult.Value);
        Assert.Equal("database.error", responseError.Code);
    }
}
