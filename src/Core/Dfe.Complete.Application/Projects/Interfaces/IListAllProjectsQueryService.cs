using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjects(ProjectState? projectStatus, ProjectType? type, bool? includeFormAMat);
}