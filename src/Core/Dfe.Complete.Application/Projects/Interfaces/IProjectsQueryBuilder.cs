using System.Linq.Expressions;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IProjectsQueryBuilder
{
    IProjectsQueryBuilder ApplyProjectFilters(ProjectFilters filters);
    IProjectsQueryBuilder ApplyGiasEstablishmentFilters(ProjectFilters filters);
    IProjectsQueryBuilder Search(string? searchTerm);
    IProjectsQueryBuilder Where(Expression<Func<Project, bool>> predicate);
    IQueryable<Project> GetProjects();
    IQueryable<ListAllProjectsQueryModel> GenerateQuery(OrderProjectQueryBy? orderBy = null);
}
