using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsForLocalAuthorityQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjectsForLocalAuthority(string localAuthorityCode, ProjectState? projectStatus, ProjectType? type);
}