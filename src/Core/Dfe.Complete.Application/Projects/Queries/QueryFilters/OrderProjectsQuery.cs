using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class OrderProjectsQuery(OrderProjectQueryBy? orderBy) : IQueryObject<Project>
{
    public IQueryable<Project> Apply(IQueryable<Project> query)
    {
        var field = orderBy?.Field ?? OrderProjectByField.CompletedAt;
        var direction = orderBy?.Direction ?? OrderByDirection.Ascending;

        IQueryable<Project> sorted = field switch
        {
            OrderProjectByField.SignificantDate =>
                direction == OrderByDirection.Ascending
                    ? query.OrderBy(p => p.SignificantDate)
                    : query.OrderByDescending(p => p.SignificantDate),

            OrderProjectByField.CreatedAt =>
                direction == OrderByDirection.Ascending
                    ? query.OrderBy(p => p.CreatedAt)
                    : query.OrderByDescending(p => p.CreatedAt),

            OrderProjectByField.CompletedAt =>
                direction == OrderByDirection.Ascending
                    ? query.OrderBy(p => p.CompletedAt)
                    : query.OrderByDescending(p => p.CompletedAt),

            _ =>
                direction == OrderByDirection.Ascending
                    ? query.OrderBy(p => p.Urn.Value)
                    : query.OrderByDescending(p => p.Urn.Value),
        };

        return sorted;
    }
}