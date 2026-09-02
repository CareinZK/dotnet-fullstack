using CSharpFunctionalExtensions;
using DirectoryService.Domain.Common;
using DirectoryService.Presentation.Common;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DirectoryService.Tests;

public sealed class ResultMappingTests
{
    [Fact]
    public void ToEnvelopeResult_Success_ReturnsOk200WithResultInEnvelope()
    {
        var result = Result.Success<string, ErrorList>("test-data");
        var envelopeResult = result.ToEnvelopeResult();

        Assert.Equal(StatusCodes.Status200OK, envelopeResult.StatusCode);
        var envelope = Assert.IsType<Envelope<string>>(envelopeResult.Envelope);
        Assert.Equal("test-data", envelope.Result);
        Assert.Null(envelope.Errors);
    }

    [Fact]
    public void ToCreatedEnvelopeResult_Success_ReturnsCreated201WithResultInEnvelope()
    {
        var id = Guid.NewGuid();
        var result = Result.Success<Guid, ErrorList>(id);
        var envelopeResult = result.ToCreatedEnvelopeResult();

        Assert.Equal(StatusCodes.Status201Created, envelopeResult.StatusCode);
        var envelope = Assert.IsType<Envelope<Guid>>(envelopeResult.Envelope);
        Assert.Equal(id, envelope.Result);
        Assert.Null(envelope.Errors);
    }

    [Fact]
    public void ToEnvelopeResult_UnitSuccess_ReturnsOk200WithNullResultInEnvelope()
    {
        var result = UnitResult.Success<ErrorList>();
        var envelopeResult = result.ToEnvelopeResult();

        Assert.Equal(StatusCodes.Status200OK, envelopeResult.StatusCode);
        var envelope = Assert.IsType<Envelope>(envelopeResult.Envelope);
        Assert.Null(envelope.Result);
        Assert.Null(envelope.Errors);
    }

    [Fact]
    public void ToEnvelopeResult_ValidationError_ReturnsBadRequest400WithErrorsInEnvelope()
    {
        var errors = new ErrorList([
            Error.Validation("validation.name", "Name required", "Name"),
            Error.Validation("validation.address", "Address required", "Address")
        ]);
        var result = Result.Failure<string, ErrorList>(errors);

        var envelopeResult = result.ToEnvelopeResult();
        Assert.Equal(StatusCodes.Status400BadRequest, envelopeResult.StatusCode);

        var envelope = Assert.IsType<Envelope<string>>(envelopeResult.Envelope);
        Assert.Null(envelope.Result);
        Assert.NotNull(envelope.Errors);
        Assert.Equal(2, envelope.Errors.Count);
        Assert.Equal("validation.name", envelope.Errors[0].Code);
        Assert.Equal("validation.address", envelope.Errors[1].Code);
    }

    [Fact]
    public void ToEnvelopeResult_NotFoundError_ReturnsNotFound404WithErrorInEnvelope()
    {
        var error = Errors.Department.NotFound(Guid.NewGuid());
        var result = Result.Failure<string, ErrorList>(error.ToErrorList());

        var envelopeResult = result.ToEnvelopeResult();
        Assert.Equal(StatusCodes.Status404NotFound, envelopeResult.StatusCode);

        var envelope = Assert.IsType<Envelope<string>>(envelopeResult.Envelope);
        Assert.Null(envelope.Result);
        Assert.NotNull(envelope.Errors);
        Assert.Single(envelope.Errors);
        Assert.Equal("department.not.found", envelope.Errors[0].Code);
    }

    [Fact]
    public void ToEnvelopeResult_ConflictError_ReturnsConflict409WithErrorInEnvelope()
    {
        var error = Errors.Location.AlreadyExists("Main");
        var result = Result.Failure<string, ErrorList>(error.ToErrorList());

        var envelopeResult = result.ToEnvelopeResult();
        Assert.Equal(StatusCodes.Status409Conflict, envelopeResult.StatusCode);

        var envelope = Assert.IsType<Envelope<string>>(envelopeResult.Envelope);
        Assert.Null(envelope.Result);
        Assert.NotNull(envelope.Errors);
        Assert.Single(envelope.Errors);
        Assert.Equal("location.already.exists", envelope.Errors[0].Code);
    }

    [Fact]
    public void ToEnvelopeResult_FailureError_ReturnsInternalServerError500WithErrorInEnvelope()
    {
        var error = Error.Failure("database.error", "Database connection lost.");
        var result = Result.Failure<string, ErrorList>(error.ToErrorList());

        var envelopeResult = result.ToEnvelopeResult();
        Assert.Equal(StatusCodes.Status500InternalServerError, envelopeResult.StatusCode);

        var envelope = Assert.IsType<Envelope<string>>(envelopeResult.Envelope);
        Assert.Null(envelope.Result);
        Assert.NotNull(envelope.Errors);
        Assert.Single(envelope.Errors);
        Assert.Equal("database.error", envelope.Errors[0].Code);
    }

    [Fact]
    public void ToEnvelopeResult_DirectError_ReturnsEnvelopeResultWithMappedStatusCode()
    {
        var error = Error.Validation("validation.field", "Field is invalid", "field");
        var envelopeResult = error.ToEnvelopeResult();

        Assert.Equal(StatusCodes.Status400BadRequest, envelopeResult.StatusCode);
        var envelope = Assert.IsType<Envelope>(envelopeResult.Envelope);
        Assert.Null(envelope.Result);
        Assert.NotNull(envelope.Errors);
        Assert.Single(envelope.Errors);
        Assert.Equal("validation.field", envelope.Errors[0].Code);
    }
}
