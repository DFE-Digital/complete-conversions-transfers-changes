using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjectsForLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsForLAQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange 
        var localAuthorityCode = fixture.Create<string>();
        const int expectedProjectsWithLaCodeCount = 10;

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var takenProjects = listAllProjectsQueryModels
            .Take(expectedProjectsWithLaCodeCount)
            .ToList();

        takenProjects.ForEach(model => model.Establishment!.LocalAuthorityCode = localAuthorityCode);
        
        var expectedProjectIds = takenProjects.Select(x => x.Project.Id);
        
        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode), default);

        Assert.NotNull(handlerResult.Value);
        Assert.Equal(expectedProjectsWithLaCodeCount, handlerResult.ItemCount);

        var actualProjectIds = handlerResult.Value.Select(x => x.ProjectId);
        Assert.All(expectedProjectIds, expectedId =>
        {
            Assert.Contains(expectedId, actualProjectIds);
        });
    }
    
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange 
        var localAuthorityCode = fixture.Create<string>();
        const int expectedProjectsWithLaCodeCount = 10;

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var takenProjects = listAllProjectsQueryModels
            .Take(expectedProjectsWithLaCodeCount)
            .ToList();

        takenProjects.ForEach(model => model.Establishment!.LocalAuthorityCode = localAuthorityCode);
        
        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode) { Page = 10 }, default);

        //Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);
    }
    
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        // Arrange
        var errorMessage = "This is a test";
        
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(new ListAllProjectsForLocalAuthorityQuery("123"), default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
    
}