namespace Dfe.Complete.Application.Common.Models;

public class PaginatedResult<T>(T value, bool isSuccess, string? error, int? itemCount) : Result<T>(value, isSuccess, error)
{
    public static PaginatedResult<T> Success(T value, int itemCount) => new(value, true, null, itemCount);
    public new static PaginatedResult<T> Failure(string error) => new(default!, false, error, null);
    public int ItemCount { get; set; } = itemCount ?? default;
};