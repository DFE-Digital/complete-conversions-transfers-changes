using System.Linq.Expressions;
using AutoFixture;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MockQueryable;
using NSubstitute;

public class ListAllProjectLocalAuthoritiesArrangementCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var localAuthorities = CreateLocalAuthorities(fixture, count: 20);
        var expectedLocalAuthorityCodes = localAuthorities.Select(la => la.Code).ToList();

        SetupLocalAuthoritiesRepository(fixture, localAuthorities);

        var projects = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
        AssignRandomLocalAuthorityCodes(projects, expectedLocalAuthorityCodes, assignCount: 20);

        SetupProjectsQueryService(fixture, projects);

        var expectedProjects = projects
            .Where(p => expectedLocalAuthorityCodes.Contains(p.Establishment.LocalAuthorityCode))
            .ToList();

        var expectedLocalAuthorities = localAuthorities
            .Where(la => expectedLocalAuthorityCodes.Contains(la.Code))
            .Select(la => new ListAllProjectLocalAuthoritiesResultModel(
                la,
                la.Code,
                expectedProjects.Count(p =>
                    p.Establishment.LocalAuthorityCode == la.Code &&
                    p.Project.Type == ProjectType.Conversion),
                expectedProjects.Count(p =>
                    p.Establishment.LocalAuthorityCode == la.Code &&
                    p.Project.Type == ProjectType.Transfer)))
            .ToList();

        fixture.Inject(localAuthorities);
        fixture.Inject(expectedLocalAuthorityCodes);
        fixture.Inject(projects);
        fixture.Inject(expectedProjects);
        fixture.Inject(expectedLocalAuthorities);
    }

    private static List<LocalAuthority> CreateLocalAuthorities(IFixture fixture, int count)
    {
        return fixture.CreateMany<LocalAuthority>(count).ToList();
    }

    private static void SetupLocalAuthoritiesRepository(IFixture fixture, List<LocalAuthority> localAuthorities)
    {
        var localAuthoritiesRepo = fixture.Freeze<ICompleteRepository<LocalAuthority>>();
        localAuthoritiesRepo
            .FetchAsync(Arg.Any<Expression<Func<LocalAuthority, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(localAuthorities.ToList());
    }

    private static void AssignRandomLocalAuthorityCodes(
        List<ListAllProjectsQueryModel> projects,
        List<string> expectedLocalAuthorityCodes,
        int assignCount)
    {
        projects
            .Take(assignCount)
            .ToList()
            .ForEach(p =>
                p.Establishment.LocalAuthorityCode = expectedLocalAuthorityCodes.MinBy(_ => Guid.NewGuid()));
    }

    private static void SetupProjectsQueryService(IFixture fixture, List<ListAllProjectsQueryModel> projects)
    {
        var projectQueryService = fixture.Freeze<IListAllProjectsQueryService>();
        var mockProjects = projects.BuildMock();
        projectQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(mockProjects);
    }
}
