using Moq;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Pages.Projects.List.CompletedProjects;
using MediatR;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;

public class AllCompletedProjectsViewModelTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task OnGet_ShouldPopulateProjectsAndPagination(
        IFixture fixture,
        [Frozen] Mock<ISender> senderMock,
        AllCompletedProjectsViewModel sut)
    {
        // ARRANGE
        // Generate some random test data for the projects
        var sampleProjects = fixture.CreateMany<ListAllProjectsResultModel>(5).ToList();

        // Suppose the "Result<T>" is how you wrap responses:
        var listResponse = Result<List<ListAllProjectsResultModel>>.Success(sampleProjects);
        var countResponse = Result<int>.Success(42);

        // Set up the mock so that sending our specific queries returns the above data
        senderMock
            .Setup(s => s.Send(It.IsAny<ListAllProjectsQuery>(), default))
            .ReturnsAsync(listResponse);

        senderMock
            .Setup(s => s.Send(It.IsAny<CountAllProjectsQuery>(), default))
            .ReturnsAsync(countResponse);

        // Typically, you'd set PageNumber / PageSize if needed:
        sut.PageNumber = 2;

        // ACT
        await sut.OnGet();

        // ASSERT
        Assert.NotNull(sut.Projects);
        Assert.Equal(5, sut.Projects.Count);
        Assert.NotNull(sut.Pagination);
        senderMock.Verify(s => s.Send(It.IsAny<ListAllProjectsQuery>(), default), Times.Once);
        senderMock.Verify(s => s.Send(It.IsAny<CountAllProjectsQuery>(), default), Times.Once);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task OnGetMovePage_ShouldCallOnGet(
        IFixture fixture,
        [Frozen] Mock<ISender> senderMock,
        AllCompletedProjectsViewModel sut)
    {
        // ARRANGE
        senderMock
            .Setup(s => s.Send(It.IsAny<ListAllProjectsQuery>(), default))
            .ReturnsAsync(Result<List<ListAllProjectsResultModel>>.Success(fixture.CreateMany<ListAllProjectsResultModel>(3).ToList()));

        senderMock
            .Setup(s => s.Send(It.IsAny<CountAllProjectsQuery>(), default))
            .ReturnsAsync(Result<int>.Success(10));

        // ACT
        await sut.OnGetMovePage();

        // ASSERT
        // If OnGetMovePage() calls OnGet(), then the mock will have been hit for both queries
        senderMock.Verify(s => s.Send(It.IsAny<ListAllProjectsQuery>(), default), Times.Once);
        senderMock.Verify(s => s.Send(It.IsAny<CountAllProjectsQuery>(), default), Times.Once);
    }
}
