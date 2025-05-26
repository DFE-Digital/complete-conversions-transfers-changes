using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsQueryModel(Project Project, GiasEstablishment? Establishment);

public record ProjectFilters(
    ProjectState? ProjectStatus,
    ProjectType? ProjectType,
    AssignedToState? AssignedToState = null,
    UserId? AssignedToUserId = null,
    UserId? CreatedByUserId = null,
    string? LocalAuthorityCode = "",
    Region? Region = null,
    ProjectTeam? Team = null,
    bool? IsFormAMat = null,
    string? NewTrustReferenceNumber = ""
);