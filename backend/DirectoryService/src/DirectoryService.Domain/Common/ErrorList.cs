using System.Collections;

namespace DirectoryService.Domain.Common;

public sealed class ErrorList : IReadOnlyList<Error>
{
    private readonly List<Error> _errors;

    public ErrorList(Error error)
    {
        _errors = [error];
    }

    public ErrorList(IEnumerable<Error> errors)
    {
        _errors = errors.ToList();
    }

    public ErrorList()
    {
        _errors = [];
    }

    public int Count => _errors.Count;

    public Error this[int index] => _errors[index];

    public IEnumerator<Error> GetEnumerator() => _errors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator ErrorList(Error error) => new(error);

    public static implicit operator ErrorList(List<Error> errors) => new(errors);

    public static implicit operator ErrorList(Error[] errors) => new(errors);
}
