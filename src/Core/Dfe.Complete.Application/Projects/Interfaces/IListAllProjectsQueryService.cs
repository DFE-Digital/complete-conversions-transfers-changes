using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjects(ProjectState? projectStatus,
        ProjectType? projectType,
        UserId? assignedToUserId = null,
        UserId? createdByUserId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? team = null
        );
}