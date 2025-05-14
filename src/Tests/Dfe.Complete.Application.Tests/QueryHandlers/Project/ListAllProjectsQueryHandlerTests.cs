using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using MockQueryable;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;
public class ListAllProjectsQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
            item.Project!,
            item.Establishment
         )).Take(20).ToList();

        var query = new ListAllProjectsQuery(null, null);

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Returns(mock);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Count, result.Value?.Count);

        for (int i = 0; i < result.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], result.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_ForOtherPages(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                item.Project,
                item.Establishment
            )).Skip(20).Take(20).ToList();

        var query = new ListAllProjectsQuery(null, null, null, 1);

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Returns(mock);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Count, result.Value?.Count);

        for (int i = 0; i < result.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], result.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldOnlyReturnProjectsThatAreAssigned(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var totalProjects = 15;
        var expectedProjects = 10;

        var combinedProjects = fixture.CreateMany<ListAllProjectsQueryModel>(totalProjects).ToList();

        // Set first 10 as assigned, last 5 as unassigned
        for (int i = 0; i < combinedProjects.Count; i++)
        {
            if (i < 10)
            {
                combinedProjects[i].Project!.AssignedTo = fixture.Create<Domain.Entities.User>();
            }
            else
            {
                combinedProjects[i].Project!.AssignedTo = null;
            }
        }


        var mock = combinedProjects.BuildMock();

        var query = new ListAllProjectsQuery(ProjectState.Active, ProjectType.Conversion, AssignedToState.AssignedOnly);

        mockListAllProjectsQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Returns(mock);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.All(result.Value!, project =>
            Assert.False(string.IsNullOrWhiteSpace(project.AssignedToFullName)));
        Assert.Equal(expectedProjects, result.Value!.Count);
    }


    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var query = new ListAllProjectsQuery(null, null, null, 10);

        var mock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Returns(mock);

        // Act
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
        ListAllProjectsQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";

        var query = new ListAllProjectsQuery(null, null);

        mockListAllProjectsQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldNotThrow_WhenOptionalFieldsAreNull(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var queryModel = fixture.Create<ListAllProjectsQueryModel>();

        queryModel.Project!.AssignedTo = null;
        queryModel.Project.CompletedAt = null;

        var queryModels = new List<ListAllProjectsQueryModel> { queryModel };
        var mock = queryModels.BuildMock();

        var query = new ListAllProjectsQuery(null, null);

        mockListAllProjectsQueryService
            .ListAllProjects(query.ProjectStatus, query.Type)
            .Returns(mock);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);

        var projected = result.Value!.First();
        Assert.Null(projected.AssignedToFullName);
        Assert.Null(projected.ProjectCompletionDate);
    }
}
