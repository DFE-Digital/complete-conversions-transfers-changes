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
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange create
        var localAuthorityCode = fixture.Create<string>();

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var expected = listAllProjectsQueryModels.Select(item =>
                ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(item.Project,
                    item.Establishment))
            .Skip(20).Take(20).ToList();

        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>(),
                localAuthorityCode: localAuthorityCode)
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode) { Page = 1 }, default);

        Assert.NotNull(handlerResult.Value);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(expected.Count, handlerResult.Value?.Count);

        mockListAllProjectsQueryService.Received(1).ListAllProjects(
            Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>(), localAuthorityCode: localAuthorityCode);

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
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange 
        var localAuthorityCode = fixture.Create<string>();

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>(),
                localAuthorityCode: localAuthorityCode)
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode) { Page = 10 }, default);

        //Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);

        mockListAllProjectsQueryService.Received(1).ListAllProjects(
            Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>(), localAuthorityCode: localAuthorityCode);
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
        var laCode = fixture.Create<string>();

        mockListAllProjectsQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>(), localAuthorityCode: laCode)
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(laCode), default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
    
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldMaintainOrdering_WhenProjectsAreOrderedBySignificantDate(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange 
        var localAuthorityCode = fixture.Create<string>();
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
        
        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>(),
                localAuthorityCode: localAuthorityCode, orderBy: Arg.Any<OrderProjectQueryBy>())
            .Returns(listAllProjectsMock);
        
        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode) { Page = 10 }, default);
        
        // Assert
        Assert.NotNull(handlerResult.Value);
        Assert.True(handlerResult.IsSuccess);
        
        var resultDates = handlerResult.Value!.Select(x => x.ConversionOrTransferDate).ToList();
        
        var orderedDates = resultDates.OrderBy(d => d).ToList();
        Assert.True(resultDates.SequenceEqual(orderedDates), 
            "Dates in the result should be in ascending order");
    }
}