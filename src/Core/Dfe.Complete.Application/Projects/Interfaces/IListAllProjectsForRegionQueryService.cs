using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsForRegionQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjectsForRegion(Region region, ProjectState? projectStatus, ProjectType? type);
}