namespace DirectoryService.Application.Exceptions;

public sealed class LocationAlreadyExistsException : Exception
{
    public LocationAlreadyExistsException()
    {
    }

    public LocationAlreadyExistsException(string message)
        : base(message)
    {
    }

    public LocationAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}