using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Enums;

namespace Dfe.Complete.Infrastructure.Extensions;

public record OrderProjectByQuery(OrderProjectByField Field = OrderProjectByField.SignificantDate, bool Ascending = false);

public static class IQueryableProjectExtensions
{
    public static IOrderedQueryable<Project> OrderProjectBy(this IQueryable<Project> query, OrderProjectByQuery? orderBy = null)
    {
        if (orderBy != null)
        {
            if (orderBy.Field == OrderProjectByField.CreatedAt && orderBy.Ascending) return query.OrderBy(p => p.CreatedAt);
            if (orderBy.Field == OrderProjectByField.CreatedAt) return query.OrderByDescending(p => p.CreatedAt);
            if (orderBy.Field == OrderProjectByField.SignificantDate && orderBy.Ascending) return query.OrderBy(p => p.SignificantDate);
        }

        return query.OrderByDescending(p => p.SignificantDate);
    }
}