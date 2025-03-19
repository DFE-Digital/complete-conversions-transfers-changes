namespace Dfe.Complete.Utils;

public class NotFoundException : Exception
{
    public string? Field { get; init; }
    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
    public NotFoundException(string message, string field)
        : base(message)
    {
        Field = field;
    }

    public NotFoundException(string message, string field, Exception innerException)
        : base(message, innerException)
    {
        Field = field;
    }
}