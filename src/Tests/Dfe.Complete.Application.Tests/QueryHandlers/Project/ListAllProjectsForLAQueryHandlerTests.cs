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

public class ListAllProjectsForLAQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsForLocalAuthorityQueryService mockListAllProjectsForLaQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange create
        var localAuthorityCode = fixture.Create<string>();

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var expected = listAllProjectsQueryModels.Select(item =>
                ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(item.Project, item.Establishment))
            .Skip(20).Take(20).ToList();

        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsForLaQueryService
            .ListAllProjectsForLocalAuthority(localAuthorityCode, Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode) { Page = 1 }, default);

        Assert.NotNull(handlerResult.Value);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(expected.Count, handlerResult.Value?.Count);

        mockListAllProjectsForLaQueryService.Received(1).ListAllProjectsForLocalAuthority(localAuthorityCode,
            Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>());
        
        for (int i = 0; i < handlerResult.Value!.Count; i++)
        {
            Assert.Equivalent(expected[i], handlerResult.Value![i]);
        }
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IListAllProjectsForLocalAuthorityQueryService mockListAllProjectsForLaQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange 
        var localAuthorityCode = fixture.Create<string>();

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsForLaQueryService
            .ListAllProjectsForLocalAuthority(localAuthorityCode, Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode) { Page = 10 }, default);

        //Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);

        mockListAllProjectsForLaQueryService.Received(1).ListAllProjectsForLocalAuthority(
            localAuthorityCode, Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>());
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IListAllProjectsForLocalAuthorityQueryService mockListAllProjectsForLaQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        // Arrange
        var errorMessage = "This is a test";
        var laCode = fixture.Create<string>();

        mockListAllProjectsForLaQueryService
            .ListAllProjectsForLocalAuthority(laCode, Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(laCode), default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
}