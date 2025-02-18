namespace Dfe.Complete.Utils;

public static class QueryableExtensions
{
    /// <summary>
    /// Applies pagination to the queryable source.
    /// </summary>
    /// <typeparam name="T">The type of elements in the source.</typeparam>
    /// <param name="source">The source queryable.</param>
    /// <param name="page">The page number (zero-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A queryable with the appropriate Skip and Take applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if page is negative or pageSize is not greater than zero.
    /// </exception>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int pageSize)
    {
        if (page < 0)
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be non-negative");
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero");

        return source.Skip(page * pageSize).Take(pageSize);
    }
}