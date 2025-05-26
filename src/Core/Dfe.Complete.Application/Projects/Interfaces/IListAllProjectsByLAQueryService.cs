using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsWithLAsQueryService
{
    IQueryable<Project> ListAllProjects(
        ProjectState? projectStatus = null,
        ProjectType? projectType = null
    );
}