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

public class ListAllProjectsForRegionQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForRegionQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var requestedRegion = Region.London;

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
        listAllProjectsQueryModels.ForEach(p => p.Project.Region = requestedRegion);

        var expected = listAllProjectsQueryModels.Select(item => new ListAllProjectsResultModel(
            item.Establishment.Name,
            item.Project.Id,
            item.Project.Urn,
            item.Project.SignificantDate,
            item.Project.State,
            item.Project.Type,
            item.Project.IncomingTrustUkprn == null,
            item.Project.AssignedTo != null
                ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                : null)).Take(20).ToList();

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(mock);

        var query = new ListAllProjectsForRegionQuery(requestedRegion, ProjectState.Active, null);

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
        ListAllProjectsForRegionQueryHandler handler,
        IFixture fixture)
    {
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(mock);

        var query = new ListAllProjectsForRegionQuery(Region.London, ProjectState.Active, null)
        {
            Page = 10
        };

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
        [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
        ListAllProjectsForRegionQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";

        var query = new ListAllProjectsForRegionQuery(null, null, null);

        mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
}