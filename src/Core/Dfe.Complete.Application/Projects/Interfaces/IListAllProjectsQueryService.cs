using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjects(
        ProjectFilters filters,
        string? search = "",
        OrderProjectQueryBy? orderBy = null
    );
}