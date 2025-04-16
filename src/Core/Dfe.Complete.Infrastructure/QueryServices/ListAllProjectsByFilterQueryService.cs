using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Dfe.Complete.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsByFilterQueryService(CompleteContext context) : IListAllProjectsByFilterQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjectsByFilter(ProjectState? projectStatus,
        ProjectType? projectType,
        AssignedToState? assignedToState = null,
        UserId? assignedToUserId = null,
        UserId? createdByUserId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? team = null,
        OrderProjectQueryBy? orderBy = null)
    {
        var projects = context.Projects
            .Where(project => projectStatus == null || project.State == projectStatus)
            .Where(project => projectType == null || projectType == project.Type);

        if (assignedToState == AssignedToState.AssignedOnly)
            projects = projects.Where(project => project.AssignedToId != null);

        IQueryable<GiasEstablishment> giasEstablishments = context.GiasEstablishments;

        if (assignedToUserId != null && assignedToUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.AssignedToId != null && project.AssignedToId == assignedToUserId);
            return GenerateQuery(projects, giasEstablishments);
        }
        
        if (createdByUserId != null && createdByUserId.Value != Guid.Empty)
        {
            projects = projects.Where(project => project.RegionalDeliveryOfficerId != null && project.RegionalDeliveryOfficerId == createdByUserId);
            return GenerateQuery(projects, giasEstablishments);
        }

        if (!string.IsNullOrEmpty(localAuthorityCode))
            giasEstablishments = giasEstablishments.Where(establishment => establishment.LocalAuthorityCode == localAuthorityCode);

        if (region != null)
            projects = projects.Where(project => project.Region == region);

        if (team != null)
            projects = projects.Where(project => project.Team == team);

        return GenerateQuery(projects, giasEstablishments, orderBy);
    }

    private static IQueryable<ListAllProjectsQueryModel> GenerateQuery(IQueryable<Project> projects, IQueryable<GiasEstablishment> giasEstablishments, OrderProjectQueryBy? orderBy = null)
    {
        return projects
            .Include(p => p.AssignedTo)
            .Include(p => p.LocalAuthority)
            .OrderProjectBy(orderBy)
            .Join(giasEstablishments, project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));
    }
}