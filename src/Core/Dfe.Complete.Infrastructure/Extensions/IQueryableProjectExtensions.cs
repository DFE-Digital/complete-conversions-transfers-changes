using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Infrastructure.Extensions;

public static class IQueryableProjectExtensions
{
    public static IOrderedQueryable<Project> OrderProjectBy(
        this IQueryable<Project> src,
        OrderProjectQueryBy? order = null)
    {
        order ??= new(OrderProjectByField.SignificantDate, OrderByDirection.Ascending);

        return (order.Field, order.Direction) switch
        {
            (OrderProjectByField.CreatedAt, OrderByDirection.Ascending) => src.OrderBy(p => p.CreatedAt),
            (OrderProjectByField.CreatedAt, _) => src.OrderByDescending(p => p.CreatedAt),

            (OrderProjectByField.CompletedAt, OrderByDirection.Ascending) => src.OrderBy(p => p.CompletedAt),
            (OrderProjectByField.CompletedAt, _) => src.OrderByDescending(p => p.CompletedAt),

            (OrderProjectByField.SignificantDate, OrderByDirection.Descending) => src.OrderByDescending(p => p.SignificantDate),

            _ => src.OrderBy(p => p.SignificantDate)
        };
    }
}