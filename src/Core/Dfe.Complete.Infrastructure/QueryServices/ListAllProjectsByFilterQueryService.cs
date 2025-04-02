using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

public enum FilterField
{
    Urn,
    SignificantDate,
    CreatedAt
}

public record OrderByQuery(FilterField Field = FilterField.Urn, bool Ascending = false);


public static class ProjectOrderingHelper
{
    public static IOrderedQueryable<Project> ApplyOrdering(this IQueryable<Project> query, FilterField field, bool descending = false)
    {
        return (field, descending) switch
        {
            (FilterField.CreatedAt, false) => query.OrderBy(p => p.CreatedAt),
            (FilterField.CreatedAt, true) => query.OrderByDescending(p => p.CreatedAt),

            (FilterField.Urn, false) => query.OrderBy(p => p.Urn),
            (FilterField.Urn, true) => query.OrderByDescending(p => p.Urn),

            (_, false) => query.OrderBy(p => p.SignificantDate),
            (_, true) => query.OrderByDescending(p => p.SignificantDate),
        };
    }
}


internal class ListAllProjectsByFilterQueryService(CompleteContext context) : IListAllProjectsByFilterQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjectsByFilter(ProjectState? projectStatus,
        ProjectType? projectType,
        UserId? userId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? team = null)
    {
        var orderBy = new OrderByQuery(FilterField.Urn, true);
        var projects = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => projectStatus != ProjectState.Active || project.AssignedToId != null)
            .Where(project => projectType == null || projectType == project.Type);

        //For now, limiting the service to one filter at a time unless requirement changes
        IQueryable<GiasEstablishment> giasEstablishments = context.GiasEstablishments;

        if (userId != null && userId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.AssignedToId != null && project.AssignedToId == userId);
            return GenerateQuery(projects, giasEstablishments);
        }

        if (!string.IsNullOrEmpty(localAuthorityCode))
        {
            giasEstablishments = giasEstablishments.Where(establishment => establishment.LocalAuthorityCode == localAuthorityCode);
            return GenerateQuery(projects, giasEstablishments);
        }

        if (region != null)
        {
            projects = projects.Where(project => project.Region == region);
            return GenerateQuery(projects, giasEstablishments, orderBy);
        }

        if (team != null)
        {
            projects = projects.Where(project => project.Team == team);
            return GenerateQuery(projects, giasEstablishments);
        }

        return GenerateQuery(projects, giasEstablishments);
    }

    private static IQueryable<ListAllProjectsQueryModel> GenerateQuery(IQueryable<Project> projects, IQueryable<GiasEstablishment> giasEstablishments, OrderByQuery? orderBy = default)
    {
        // return projects
        //     .Include(p => p.AssignedTo)
        //     .Include(p => p.LocalAuthority)
        //     // .OrderBy(p => p.SignificantDate)
        //             .Join(giasEstablishments, project => project.Urn, establishment => establishment.Urn,
        //         (project, establishment) => new ListAllProjectsQueryModel(project, establishment));
        var withRelations = projects
            .Include(p => p.AssignedTo)
            .Include(p => p.LocalAuthority);

        IOrderedQueryable<Project> orderedByA;

        if (orderBy != null)
        {
            orderedByA = withRelations.ApplyOrdering(orderBy.Field, orderBy.Ascending);
        }
        else orderedByA = withRelations.OrderBy(p => p.SignificantDate);

        return orderedByA
            .Join(giasEstablishments, project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));
    }
}