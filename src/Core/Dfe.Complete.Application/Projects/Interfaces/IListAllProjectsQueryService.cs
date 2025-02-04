using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjects(ProjectState? projectStatus, ProjectType? type);
}