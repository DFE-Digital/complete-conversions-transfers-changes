using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
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
        var orderBy = new OrderProjectQueryBy() { Field = OrderProjectByField.SignificantDate, Direction = OrderByDirection.Ascending };

        mockListAllProjectsQueryService
            .ListAllProjects(new ProjectFilters(ProjectState.Active, ProjectType.Conversion, WithAcademyUrn: true), null, orderBy)
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
        
        var orderBy = new OrderProjectQueryBy() { Field = OrderProjectByField.SignificantDate, Direction = OrderByDirection.Ascending };

        mockListAllProjectsQueryService
            .ListAllProjects(new ProjectFilters(ProjectState.Active, ProjectType.Conversion, WithAcademyUrn: false), null, orderBy)
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
    
    [Theory]
    [CustomAutoData]
    public async Task Handle_ShouldUseTaskDataAcademyDetailsName_WhenAvailable(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        [Frozen] ICompleteRepository<GiasEstablishment> mockEstablishmentRepo,
        [Frozen] ICompleteRepository<ConversionTasksData> mockTaskDataRepo,
        ListAllProjectsConvertingQueryHandler handler)
    {
        // Arrange
        var taskDataId = new TaskDataId(Guid.NewGuid());
        var academyUrn = new Urn(123456);
        var now = DateTime.UtcNow;

        var project = new Domain.Entities.Project
        {
            Id = new ProjectId(Guid.NewGuid()),
            TasksDataId = taskDataId,
            AcademyUrn = academyUrn,
            SignificantDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var establishment = new GiasEstablishment()
        {
            Name = "Some Establishment",
            Urn = new Urn(999999)
        };

        var projectModel = new ListAllProjectsQueryModel(project, establishment);

        var mockQueryable = new List<ListAllProjectsQueryModel> { projectModel }.BuildMock();
        var orderBy = new OrderProjectQueryBy { Field = OrderProjectByField.SignificantDate, Direction = OrderByDirection.Ascending };

        mockListAllProjectsQueryService
            .ListAllProjects(
                new ProjectFilters(ProjectState.Active, ProjectType.Conversion, WithAcademyUrn: true),
                null,
                orderBy)
            .Returns(mockQueryable);

        mockTaskDataRepo
            .FetchAsync(Arg.Any<Expression<Func<ConversionTasksData, bool>>>())
            .Returns(new List<ConversionTasksData>
            {
                new (taskDataId, now, now)
                {
                    AcademyDetailsName = "Academy Name From TaskData"
                }
            });

        mockEstablishmentRepo
            .FetchAsync(Arg.Any<Expression<Func<GiasEstablishment, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new List<GiasEstablishment>
            {
                new GiasEstablishment
                {
                    Urn = academyUrn,
                    Name = "Academy Name From GIAS"
                }
            });

        var request = new ListAllProjectsConvertingQuery(WithAcademyUrn: true) { Page = 0, Count = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var expectedProject = result.Value?.Single();
        Assert.Equal("Academy Name From TaskData", expectedProject?.AcademyName);
    }

}
