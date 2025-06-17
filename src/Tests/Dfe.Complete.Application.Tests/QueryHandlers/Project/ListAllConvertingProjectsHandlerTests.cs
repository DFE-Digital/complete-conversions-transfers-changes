using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsConvertingQueryHandlerTests
{
    [Theory]
    [CustomAutoData]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsConvertingQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is an error";
        mockListAllProjectsQueryService
            .ListAllProjects(new ProjectFilters(ProjectState.Active, ProjectType.Conversion, WithAcademyUrn: true))
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(new ListAllProjectsConvertingQuery(WithAcademyUrn: true) { Page = 0, Count = 10 }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }

    [Theory]
    [CustomAutoData(typeof(DateOnlyCustomization), typeof(ListAllProjectsQueryModelCustomization), typeof(IgnoreVirtualMembersCustomisation))]
    public async Task Handle_ShouldReturnPaginatedConvertingProjects(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsConvertingQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var projects = fixture.CreateMany<ListAllProjectsQueryModel>(20).ToList();
        foreach (var project in projects)
        {
            project.Project.AcademyUrn = null;
            project.Project.CompletedAt = null;
            project.Project.SignificantDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }        

        var mockQueryable = projects.BuildMock();
        mockListAllProjectsQueryService
            .ListAllProjects(new ProjectFilters(ProjectState.Active, ProjectType.Conversion, WithAcademyUrn: false))
            .Returns(mockQueryable);

        var request = new ListAllProjectsConvertingQuery(WithAcademyUrn: false) { Page = 1, Count = 5 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value?.Count());
        Assert.Equal(projects.Count, result.ItemCount);
    }
}
