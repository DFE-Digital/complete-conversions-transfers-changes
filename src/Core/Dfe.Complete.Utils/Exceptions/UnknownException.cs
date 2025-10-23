namespace Dfe.Complete.Utils.Exceptions;

public class UnknownException : Exception
{
    public string? Field { get; init; }
    public UnknownException(string message)
        : base(message)
    {
    }

    public UnknownException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public UnknownException(string message, string field)
        : base(message)
    {
        Field = field;
    }

    public UnknownException(string message, string field, Exception innerException)
        : base(message, innerException)
    {
        Field = field;
    }
}
