namespace Dfe.Complete.Utils.Exceptions;

public class UnprocessableContentException : Exception
{
    public string? Field { get; init; }
    public UnprocessableContentException(string message)
        : base(message)
    {
    }

    public UnprocessableContentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
    public UnprocessableContentException(string message, string field)
        : base(message)
    {
        Field = field;
    }

    public UnprocessableContentException(string message, string field, Exception innerException)
        : base(message, innerException)
    {
        Field = field;
    }
}