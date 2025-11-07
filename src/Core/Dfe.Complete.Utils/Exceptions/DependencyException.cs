namespace Dfe.Complete.Utils.Exceptions;

public class DependencyException : Exception
{
    public string? Field { get; init; }
    public DependencyException(string message)
        : base(message)
    {
    }

    public DependencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DependencyException(string message, string field)
        : base(message)
    {
        Field = field;
    }

    public DependencyException(string message, string field, Exception innerException)
        : base(message, innerException)
    {
        Field = field;
    }
}
