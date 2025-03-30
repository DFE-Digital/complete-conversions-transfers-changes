using AutoFixture;
using Dfe.Complete.Infrastructure.QueryServices;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Tests.Common.Mocks;
using Dfe.Complete.Application.Projects.Interfaces;

namespace Dfe.Complete.Application.Tests.Services.Projects;

public class ListAllProjectsByFilterQueryServiceTests
{
    public readonly IListAllProjectsByFilterQueryService _service;

    public ListAllProjectsByFilterQueryServiceTests()
    {
        var fixture = new Fixture();
        fixture.Customize(new ProjectCustomization());
        fixture.Customize(new GiasEstablishmentsCustomization());

        var (mockProjects, mockEstablishments) = CreateTestProjectData(fixture);
        var contextMock = MockCompleteContext.Create(mockProjects, mockEstablishments);
        _service = new ListAllProjectsByFilterQueryService(contextMock.Object);
    }

    [Theory]
    [InlineData(null, null, null, null, null, null, 60)]
    [InlineData(ProjectState.Completed, null, null, null, null, null, 40)]
    [InlineData(null, ProjectType.Transfer, null, null, null, null, 10)]
    [InlineData(null, null, "00000000-0000-0000-0000-000000000001", null, null, null, 10)]
    [InlineData(null, null, null, Region.London, null, null, 3)]
    [InlineData(null, null, null, null, ProjectTeam.RegionalCaseWorkerServices, null, 4)]
    [InlineData(null, null, null, null, null, "target string", 8)]
    // To show that filtering to Active projects also filters out unassigned projects
    [InlineData(ProjectState.Active, null, null, null, null, null, 5)]

    public async Task Handle_ShouldReturnProjects_WhenFiltered(
        ProjectState? expectedState,
        ProjectType? expectedType,
        string? expectedUserId,
        Region? expectedRegion,
        ProjectTeam? expectedTeam,
        string? expectedLocalAuthority,
        int expectedCount
    )
    {
        // Arrange
        UserId? expectedUserGuid = expectedUserId != null ? new UserId(new Guid(expectedUserId)) : null;

        // Act
        var result = _service
            .ListAllProjectsByFilter(
                expectedState,
                expectedType,
                userId: expectedUserGuid,
                region: expectedRegion,
                team: expectedTeam,
                localAuthorityCode: expectedLocalAuthority
            )
            .ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);

        foreach (var entry in result)
        {
            if (expectedUserGuid != null)
                Assert.Equal(expectedUserGuid, entry.Project.AssignedToId);

            if (expectedType != null)
                Assert.Equal(expectedType, entry.Project.Type);

            if (expectedRegion != null)
                Assert.Equal(expectedRegion, entry.Project.Region);

            if (expectedTeam != null)
                Assert.Equal(expectedTeam, entry.Project.Team);

            if (expectedLocalAuthority != null)
                Assert.Equal(expectedLocalAuthority, entry.Establishment.LocalAuthorityCode);
        }
    }

    [Fact]
    public void Handle_ShouldApplyUserFilter_WhenMultipleFiltersProvided()
    {
        // Arrange
       var expectedUserId = new UserId(new Guid("00000000-0000-0000-0000-000000000001"));

        // Act
        var result = _service
            .ListAllProjectsByFilter(
                null,
                null,
                userId: expectedUserId,
                region: Region.SouthEast,
                team: ProjectTeam.SouthEast,
                localAuthorityCode: "bad la code"
            )
            .ToList();

        // Assert
        Assert.Equal(10, result.Count);
        Assert.All(result, r => Assert.Equal(expectedUserId, r.Project.AssignedToId));
    }

    public static (List<Project>, List<GiasEstablishment>) CreateTestProjectData(IFixture fixture)
    {
        // Set up test data that can test all relevent filter permutations
        var targetUserId = new UserId(new Guid("00000000-0000-0000-0000-000000000001"));
        var targetUrn = fixture.Create<Urn>();
        var otherUrn = fixture.Create<Urn>();
        var projects = new List<Project>();
        var establishments = new List<GiasEstablishment>();

        // 1. With 2, userID only filter (10)
        projects.AddRange([.. fixture
            .Build<Project>()
            .With(p => p.AssignedToId, targetUserId)
            .With(p => p.Urn, otherUrn)
            .With(p => p.Type, ProjectType.Conversion)
            .With(p => p.State, ProjectState.Active)
            .With(p => p.Region, Region.NorthEast)
            .With(p => p.Team, ProjectTeam.BusinessSupport)
            .CreateMany(5)]
        );

        // 2. With 1, userId only filter (10)
        projects.AddRange([.. fixture
            .Build<Project>()
            .With(p => p.AssignedToId, targetUserId)
            .With(p => p.Urn, otherUrn)
            .With(p => p.Type, ProjectType.Conversion)
            .With(p => p.State, ProjectState.Completed)
            .With(p => p.Region, Region.NorthEast)
            .With(p => p.Team, ProjectTeam.BusinessSupport)
            .CreateMany(5)]
        );

        // 3. Region only filter (3)
        projects.AddRange([.. fixture
            .Build<Project>()
            .With(p => p.Urn, otherUrn)
            .With(p => p.Type, ProjectType.Conversion)
            .With(p => p.State, ProjectState.Completed)
            .With(p => p.Region, Region.London)
            .With(p => p.Team, ProjectTeam.BusinessSupport)
            .CreateMany(3)]
        );

        // 4. Team only filter (4)
        projects.AddRange([.. fixture
            .Build<Project>()
            .With(p => p.Urn, otherUrn)
            .With(p => p.Type, ProjectType.Conversion)
            .With(p => p.State, ProjectState.Completed)
            .With(p => p.Region, Region.NorthEast)
            .With(p => p.Team, ProjectTeam.RegionalCaseWorkerServices)
            .CreateMany(4)]
        );

        // 5. LocalAUthority only filter (8)
        projects.AddRange([.. fixture
            .Build<Project>()
            .With(p => p.Urn, targetUrn)
            .With(p => p.Type, ProjectType.Conversion)
            .With(p => p.State, ProjectState.Completed)
            .With(p => p.Region, Region.NorthEast)
            .With(p => p.Team, ProjectTeam.BusinessSupport)
            .CreateMany(8)]
        );

        // 6. Transfers only filter (10)
        projects.AddRange([.. fixture
            .Build<Project>()
            .With(p => p.Urn, otherUrn)
            .With(p => p.Type, ProjectType.Transfer)
            .With(p => p.State, ProjectState.Completed)
            .With(p => p.Region, Region.NorthEast)
            .With(p => p.Team, ProjectTeam.BusinessSupport)
            .CreateMany(10)]
        );

        // 7. In progress with no assignee (filtered out when looking for ACTIVE projects)
        projects.AddRange([.. fixture
           .Build<Project>()
           .With(p => p.AssignedToId, (UserId)null)
           .With(p => p.Urn, otherUrn)
           .With(p => p.Type, ProjectType.Conversion)
           .With(p => p.State, ProjectState.Active)
           .With(p => p.Region, Region.NorthEast)
           .With(p => p.Team, ProjectTeam.BusinessSupport)
           .CreateMany(15)]        
        );

        // 8. Completed with no assignee (NOT filtered out when looking for COMPLETED projects)
        projects.AddRange([.. fixture
           .Build<Project>()
           .With(p => p.AssignedToId, (UserId)null)
           .With(p => p.Urn, otherUrn)
           .With(p => p.Type, ProjectType.Conversion)
           .With(p => p.State, ProjectState.Completed)
           .With(p => p.Region, Region.NorthEast)
           .With(p => p.Team, ProjectTeam.BusinessSupport)
           .CreateMany(10)]
        );

        establishments.AddRange(fixture.Build<GiasEstablishment>().With(e => e.Urn, otherUrn).With(e => e.LocalAuthorityCode, "some string").CreateMany(1));
        establishments.AddRange(fixture.Build<GiasEstablishment>().With(e => e.Urn, targetUrn).With(e => e.LocalAuthorityCode, "target string").CreateMany(1));

        return (projects, establishments);
    }
}
