using CSharpFunctionalExtensions;

namespace DirectoryService.Domain.Common;

public static class ResultConversionExtensions
{
    public static Result<T, ErrorList> ToErrorList<T>(this Result<T, Error> result) =>
        result.IsSuccess
            ? Result.Success<T, ErrorList>(result.Value)
            : Result.Failure<T, ErrorList>(new ErrorList(result.Error));

    public static UnitResult<ErrorList> ToErrorList(this UnitResult<Error> result) =>
        result.IsSuccess
            ? UnitResult.Success<ErrorList>()
            : UnitResult.Failure(new ErrorList(result.Error));
}
