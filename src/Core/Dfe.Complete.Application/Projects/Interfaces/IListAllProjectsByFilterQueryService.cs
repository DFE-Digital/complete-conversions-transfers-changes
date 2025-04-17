using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsByFilterQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjectsByFilter(ProjectState? projectStatus,
        ProjectType? projectType,
        AssignedToState? assignedToState = null,
        UserId? userId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? team = null
        );
}