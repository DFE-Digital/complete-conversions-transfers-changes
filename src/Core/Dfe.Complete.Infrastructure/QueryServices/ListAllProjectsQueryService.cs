using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsQueryService(CompleteContext context) : IListAllProjectsQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjects(
        ProjectFilters filters,
        string? search = "",
        OrderProjectQueryBy? orderBy = null)
    {
        var query = new ProjectsQueryBuilder(context)
            .ApplyProjectFilters(filters)
            .ApplyGiasEstablishmentFilters(filters)
            .Search(search)
            .GenerateQuery(orderBy);

        return query;
    }
}