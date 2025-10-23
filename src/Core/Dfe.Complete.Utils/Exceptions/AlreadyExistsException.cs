namespace Dfe.Complete.Utils.Exceptions;

public class AlreadyExistsException : Exception
{
    public string? Field { get; init; }
    public AlreadyExistsException(string message)
        : base(message)
    {
    }

    public AlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public AlreadyExistsException(string message, string field)
        : base(message)
    {
        Field = field;
    }

    public AlreadyExistsException(string message, string field, Exception innerException)
        : base(message, innerException)
    {
        Field = field;
    }
}
