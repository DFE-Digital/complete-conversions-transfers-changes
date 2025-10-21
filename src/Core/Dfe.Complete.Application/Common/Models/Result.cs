using Dfe.Complete.Utils;
using Dfe.Complete.Utils.Exceptions;

namespace Dfe.Complete.Application.Common.Models;

public enum ErrorType
{
    None,
    Unauthorized,
    NotFound,
    Unknown,
    UnprocessableContent

}

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }
    public Exception? Exception { get; init; }

    protected Result(T value, bool isSuccess, string? error, ErrorType errorType = ErrorType.None)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(value, true, null);
    public static Result<T> Failure(string error, ErrorType errorType = ErrorType.Unknown)
    {
        Exception? exception = errorType switch
        {
            ErrorType.NotFound => new NotFoundException(error),
            ErrorType.UnprocessableContent => new UnprocessableContentException(error),
            _ => new UnknownException(error),
        };

        return new(default!, false, error, errorType)
        {
            Exception = exception
        };
    }
}
