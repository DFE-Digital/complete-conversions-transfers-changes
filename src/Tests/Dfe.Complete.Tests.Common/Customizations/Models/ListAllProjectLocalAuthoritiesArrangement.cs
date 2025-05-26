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

        var projects = fixture.CreateMany<Project>(50).ToList();

        SetupProjectsQueryService(fixture, projects);
        var expectedLocalAuthoritiesResult = projects.GroupBy(p => p.LocalAuthority.Code)
            .Select(g => new
            {
                LocalAuthorityCode = g.Key,
                LocalAuthorityName = g.First().LocalAuthority.Name,
                ConversionCount = g.Count(p => p.Type == ProjectType.Conversion),
                TransferCount = g.Count(p => p.Type == ProjectType.Transfer)
            }).ToList();

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

    private static void SetupProjectsQueryService(IFixture fixture, List<Project> projects)
    {
        var projectQueryService = fixture.Freeze<IListAllProjectsWithLAsQueryService>();
        var mockProjects = projects.BuildMock();
        projectQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(mockProjects);
    }
}