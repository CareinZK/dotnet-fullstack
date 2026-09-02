using DirectoryService.Domain.Common;

namespace DirectoryService.Presentation.Common;

public record Envelope
{
    public object? Result { get; }
    public IReadOnlyList<Error>? Errors { get; }
    public DateTime TimeGenerated { get; }

    public Envelope(object? result, IReadOnlyList<Error>? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope Ok(object? result = null) => new(result, null);
    public static Envelope Error(IReadOnlyList<Error> errors) => new(null, errors);
    public static Envelope Error(Error error) => new(null, [error]);
}

public sealed record Envelope<T>
{
    public T? Result { get; }
    public IReadOnlyList<Error>? Errors { get; }
    public DateTime TimeGenerated { get; }

    public Envelope(T? result, IReadOnlyList<Error>? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope<T> Ok(T? result = default) => new(result, null);
    public static Envelope<T> Error(IReadOnlyList<Error> errors) => new(default, errors);
    public static Envelope<T> Error(Error error) => new(default, [error]);
}
