using System.Linq.Expressions;
using AutoFixture;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Tests.Common.Customizations.Models;

public class ListAllProjectLocalAuthoritiesArrangement : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var localAuthorities = CreateLocalAuthorities(fixture, count: 20);
        var assignedLaCodesToProjects = localAuthorities.Select(la => la.Code).ToList();

        SetupLocalAuthoritiesRepository(fixture, localAuthorities);

        var projects = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
        SetupProjectEstablishmentsWithRandomLACode(projects, assignedLaCodesToProjects, takeCount: 20);

        SetupProjectsQueryService(fixture, projects);

        var expectedLocalAuthoritiesResult = localAuthorities.Select(la =>
                new ListAllProjectLocalAuthoritiesResultModel(
                    la,
                    la.Code,
                    projects.Count(p =>
                        p.Establishment.LocalAuthorityCode == la.Code && p.Project.Type == ProjectType.Conversion),
                    projects.Count(p =>
                        p.Establishment.LocalAuthorityCode == la.Code && p.Project.Type == ProjectType.Transfer)))
            .ToList();

        fixture.Inject(expectedLocalAuthoritiesResult);
        fixture.Inject(assignedLaCodesToProjects);
    }

    private static List<LocalAuthority> CreateLocalAuthorities(IFixture fixture, int count)
    {
        return fixture.CreateMany<LocalAuthority>(count).ToList();
    }

    private static void SetupLocalAuthoritiesRepository(IFixture fixture, List<LocalAuthority> localAuthorities)
    {
        var localAuthoritiesRepo = fixture.Freeze<ICompleteRepository<LocalAuthority>>();
        localAuthoritiesRepo
            .FetchAsync(Arg.Any<Expression<Func<LocalAuthority, bool>>>())
            .Returns(localAuthorities.ToList());
    }

    private static void SetupProjectEstablishmentsWithRandomLACode(
        List<ListAllProjectsQueryModel> projects,
        List<string> expectedLocalAuthorityCodes,
        int takeCount)
    {
        projects
            .Take(takeCount)
            .ToList()
            //Get random authority code and assign to Project Establishment
            .ForEach(p => p.Establishment.LocalAuthorityCode = expectedLocalAuthorityCodes.MinBy(_ => Guid.NewGuid()));
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