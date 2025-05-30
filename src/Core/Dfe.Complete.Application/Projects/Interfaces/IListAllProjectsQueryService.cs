using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjects(
        ProjectFilters filters,
        string? search = "",
        OrderProjectQueryBy? orderBy = null
        );

    IQueryable<Project> ListAllProjectsWithRegion(
        ProjectState? projectStatus,
        ProjectType? projectType);
}