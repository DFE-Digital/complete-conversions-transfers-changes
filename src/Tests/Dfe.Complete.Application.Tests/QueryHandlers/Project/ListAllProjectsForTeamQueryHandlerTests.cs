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

public class ListAllProjectsForTeamQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsForFilterQueryService,
        ListAllProjectsForTeamQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var requestedTeam = ProjectTeam.London;

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var expected = listAllProjectsQueryModels.Select(item =>
                ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(item.Project!, item.Establishment))
            .Skip(20).Take(20).ToList();

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsForFilterQueryService.ListAllProjects(
            Arg.Is<ProjectFilters>(f =>
                f.Team == requestedTeam))
            .Returns(mock);

        var query = new ListAllProjectsForTeamQuery(requestedTeam, ProjectState.Active, null) { Page = 1 };

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
        [Frozen] IListAllProjectsQueryService mockListAllProjectsForFilterQueryService,
        ListAllProjectsForTeamQueryHandler handler,
        IFixture fixture)
    {
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsForFilterQueryService.ListAllProjects(
            Arg.Is<ProjectFilters>(f =>
                f.Team == ProjectTeam.London))
            .Returns(mock);

        var query = new ListAllProjectsForTeamQuery(ProjectTeam.London, ProjectState.Active, null) { Page = 10 };

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
        [Frozen] IListAllProjectsQueryService mockListAllProjectsForFilterQueryService,
        ListAllProjectsForTeamQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";

        var query = new ListAllProjectsForTeamQuery(ProjectTeam.London, null, null);

        mockListAllProjectsForFilterQueryService.ListAllProjects(new ProjectFilters(query.ProjectStatus, query.Type, Team: ProjectTeam.London))
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
}