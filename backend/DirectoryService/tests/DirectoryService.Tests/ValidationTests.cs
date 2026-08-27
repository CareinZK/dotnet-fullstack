using DirectoryService.Application.Common;
using DirectoryService.Application.Departments;
using DirectoryService.Application.Locations;
using DirectoryService.Contracts;
using DirectoryService.Domain.Common;
using Xunit;

namespace DirectoryService.Tests;

public sealed class ValidationTests
{
    [Fact]
    public async Task CreateLocationDtoValidator_WithEmptyNameAndAddress_ReturnsMultipleValidationErrors()
    {
        var validator = new CreateLocationDtoValidator();
        var dto = new CreateLocationDto(string.Empty, string.Empty);

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);

        var errorList = result.ToErrorList();
        Assert.Equal(2, errorList.Count);
        Assert.All(errorList, e => Assert.Equal(ErrorType.Validation, e.Type));
        Assert.Contains(errorList, e => e.InvalidField == "Name");
        Assert.Contains(errorList, e => e.InvalidField == "Address");
    }

    [Fact]
    public async Task CreateDepartmentDtoValidator_WithInvalidSlugAndName_ReturnsValidationErrors()
    {
        var validator = new CreateDepartmentDtoValidator();
        var dto = new CreateDepartmentDto(string.Empty, "INVALID SLUG!", null, null);

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        var errorList = result.ToErrorList();
        Assert.True(errorList.Count >= 2);
        Assert.All(errorList, e => Assert.Equal(ErrorType.Validation, e.Type));
    }
}
