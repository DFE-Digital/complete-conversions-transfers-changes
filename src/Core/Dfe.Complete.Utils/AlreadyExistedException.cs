namespace Dfe.Complete.Utils
{
    public class AlreadyExistedException : Exception
    {
        public string? Field { get; init; }
        public AlreadyExistedException(string message)
            : base(message)
        {
        }

        public AlreadyExistedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AlreadyExistedException(string message, string field)
            : base(message)
        {
            Field = field;
        }

        public AlreadyExistedException(string message, string field, Exception innerException)
            : base(message, innerException)
        {
            Field = field;
        }
    }
}
