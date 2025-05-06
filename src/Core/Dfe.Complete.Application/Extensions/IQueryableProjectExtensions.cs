using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Infrastructure.Extensions;

public static class IQueryableProjectExtensions
{
    public static IOrderedQueryable<Project> OrderProjectBy(this IQueryable<Project> query, OrderProjectQueryBy? orderBy = null)
    {
        if (orderBy != null)
        {
            if (orderBy.Field == OrderProjectByField.CreatedAt && orderBy.Direction == OrderByDirection.Ascending) return query.OrderBy(p => p.CreatedAt);
            if (orderBy.Field == OrderProjectByField.CreatedAt) return query.OrderByDescending(p => p.CreatedAt);
            if (orderBy.Field == OrderProjectByField.CompletedAt && orderBy.Direction == OrderByDirection.Ascending) return query.OrderBy(p => p.CompletedAt);
            if (orderBy.Field == OrderProjectByField.CompletedAt) return query.OrderByDescending(p => p.CompletedAt);
            if (orderBy.Field == OrderProjectByField.Id && orderBy.Direction == OrderByDirection.Ascending) return query.OrderBy(p => p.Id);
            if (orderBy.Field == OrderProjectByField.Id) return query.OrderByDescending(p => p.Id);
            if (orderBy.Field == OrderProjectByField.SignificantDate && orderBy.Direction == OrderByDirection.Descending) return query.OrderByDescending(p => p.SignificantDate);
        }

        return query.OrderBy(p => p.SignificantDate);
    }
}