using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Tests.Common.Customizations.Fakes;

public class FakeProjectsQueryBuilder : IProjectsQueryBuilder
{
    private IQueryable<Project> _projects;

    public FakeProjectsQueryBuilder()
    {
        _projects = new List<Project>().AsQueryable();
    }

    public IProjectsQueryBuilder ApplyProjectFilters(ProjectFilters filters)
    {
        return this;
    }

    public IProjectsQueryBuilder ApplyGiasEstablishmentFilters(ProjectFilters filters)
    {
        return this;
    }

    public IProjectsQueryBuilder Search(string? searchTerm)
    {
        return this;
    }

    public IProjectsQueryBuilder Where(System.Linq.Expressions.Expression<System.Func<Project, bool>> predicate)
    {
        _projects = _projects.Where(predicate);
        return this;
    }

    public IQueryable<Project> GetProjects()
    {
        return _projects;
    }

    public IQueryable<ListAllProjectsQueryModel> GenerateQuery(OrderProjectQueryBy? orderBy = null)
    {
        return _projects.Select(p => new ListAllProjectsQueryModel(p, new GiasEstablishment()));
    }
}