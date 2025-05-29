using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListSearchProjects(List<ProjectState>? projectStatuses,
       string search,
       OrderProjectQueryBy? orderBy = null);

    IQueryable<ListAllProjectsQueryModel> ListAllProjects(ProjectState? projectStatus,
        ProjectType? projectType,
        AssignedToState? assignedToState = null,
        UserId? assignedToUserId = null,
        UserId? createdByUserId = null,
        string? localAuthorityCode = "",
        Region? region = null,
        ProjectTeam? team = null,
        bool? isFormAMat = null,
        string? newTrustReferenceNumber = "",
        string? incomingTrustUkprn = null,
        OrderProjectQueryBy? orderBy = null
        );
}