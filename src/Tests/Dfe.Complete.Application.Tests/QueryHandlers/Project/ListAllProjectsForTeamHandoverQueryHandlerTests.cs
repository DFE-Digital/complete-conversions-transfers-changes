using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsForTeamHandoverQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForTeamHandoverQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        const Region requestedRegion = Region.London;

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var expected = listAllProjectsQueryModels.Select(item =>
                ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(item.Project,
                    item.Establishment))
            .Skip(20).Take(20).ToList();

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(),
                Arg.Any<ProjectType?>(), region: requestedRegion, team: ProjectTeam.RegionalCaseWorkerServices)
            .Returns(mock);

        var query = new ListAllProjectsForTeamHandoverQuery(requestedRegion, ProjectState.Active, null) { Page = 1 };

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(expected.Count, result.Value?.Count);
        for (var i = 0; i < result.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], result.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForTeamHandoverQueryHandler handler,
        IFixture fixture)
    {
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(),
                Arg.Any<ProjectType?>(), region: Region.London, team: ProjectTeam.RegionalCaseWorkerServices)
            .Returns(mock);

        var query = new ListAllProjectsForTeamHandoverQuery(Region.London, ProjectState.Active, null) { Page = 10 };

        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value?.Count);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForTeamHandoverQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";

        var query = new ListAllProjectsForTeamHandoverQuery(Region.London, null, null);

        mockListAllProjectsQueryService
            .ListAllProjects(query.ProjectStatus, query.Type, region: Region.London, team: ProjectTeam.RegionalCaseWorkerServices)
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
}