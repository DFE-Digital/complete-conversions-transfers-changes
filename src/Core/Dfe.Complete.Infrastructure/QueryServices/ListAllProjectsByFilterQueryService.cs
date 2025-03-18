using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsByFilterQueryService(CompleteContext context) : IListAllProjectsByFilterQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjectsByFilter(ProjectState? projectStatus,
        ProjectType? projectType,
        UserId? userId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? projectTeam = null)
    {
        var projects = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
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
            return GenerateQuery(projects, giasEstablishments);
        }

        if (projectTeam != null)
        {
            projects = projects.Where(project => project.Team == projectTeam);
            return GenerateQuery(projects, giasEstablishments);
        }

        return GenerateQuery(projects, giasEstablishments);
    }

    private static IQueryable<ListAllProjectsQueryModel> GenerateQuery(IQueryable<Project> projects, IQueryable<GiasEstablishment> giasEstablishments)
    {
        return projects
            .Include(p => p.AssignedTo)
            .Include(p => p.LocalAuthority)
            .Join(giasEstablishments, project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));
    }
}