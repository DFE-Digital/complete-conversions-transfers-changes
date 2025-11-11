using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsByMonthHandlerTests
{
    [Theory]
    [CustomAutoData]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        ListAllProjectsByMonthsQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .Where(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .GenerateQuery()
            .Throws(new Exception(errorMessage));

        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var result = await handler.Handle(new ListProjectsByMonthsQuery(date, null, ProjectState.Active, ProjectType.Conversion), default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnUnsuccessful_WhenTrustLookupFails(
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        [Frozen] ITrustsV4Client mockTrustsClient,
        ListAllProjectsByMonthsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var expectedError = "Trust lookup error";
        var mockProjects = fixture.CreateMany<ListAllProjectsQueryModel>(3).BuildMock();

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .Where(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .GenerateQuery()
            .Returns(mockProjects);

        mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Throws(new Exception(expectedError));

        var date = DateOnly.FromDateTime(DateTime.Today);

        // Act
        var result = await handler.Handle(new ListProjectsByMonthsQuery(date, null, ProjectState.Active, ProjectType.Conversion), default);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Contains(expectedError, result.Error);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ProjectsQueryBuilderCustomization))]
    public async Task Handle_ShouldCorrectlyPaginateResults(
        [Frozen] ITrustsV4Client mockTrustsClient,
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        ListAllProjectsByMonthsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var ukprn = fixture.Create<int>();
        var dateFrom = new DateOnly(2025, 1, 1);
        var dateTo = new DateOnly(2025, 12, 31);
        var user = fixture.Create<Domain.Entities.User>();

        var listAllProjectsQueryModels = fixture.Build<ListAllProjectsQueryModel>()
            .CreateMany(50)
            .Select(p =>
            {
                p.Project.IncomingTrustUkprn = new Ukprn(ukprn);
                p.Project.SignificantDate = dateFrom;
                p.Project.SignificantDateProvisional = false;
                p.Project.AssignedTo = user;
                return p;
            })
            .BuildMock();

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .Where(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .GenerateQuery()
            .Returns(listAllProjectsQueryModels);

        var trustDtos = fixture
            .Build<TrustDto>()
            .With(t => t.Ukprn, ukprn.ToString())
            .CreateMany(1)
            .OrderBy(t => t.Name);

        mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ObservableCollection<TrustDto>(trustDtos)));

        // Act
        var result = await handler.Handle(new ListProjectsByMonthsQuery(dateFrom, dateTo, ProjectState.Active, ProjectType.Conversion, Page: 2, Count: 10), default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value?.Count);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ProjectsQueryBuilderCustomization))]
    public async Task Handle_ShouldFilterProjectsByDateRange(
        [Frozen] ITrustsV4Client mockTrustsClient,
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        ListAllProjectsByMonthsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var ukprn = fixture.Create<int>();
        var user = fixture.Create<Domain.Entities.User>();

        var startDate = new DateOnly(2025, 6, 1);
        var endDate = new DateOnly(2025, 6, 30);
        var allProjects = fixture.Build<ListAllProjectsQueryModel>()
            .CreateMany(6)
            .ToList();

        var trustDtos = fixture
            .Build<TrustDto>()
            .With(t => t.Ukprn, ukprn.ToString())
            .CreateMany(1)
            .OrderBy(t => t.Name);

        mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ObservableCollection<TrustDto>(trustDtos)));

        List<ListAllProjectsQueryModel> projects = new List<ListAllProjectsQueryModel>();

        foreach (var project in allProjects)
        {
            project.Project.SignificantDate = startDate;
            project.Project.SignificantDateProvisional = false;
            project.Project.AssignedTo = user;

            projects.Add(project);
        }

        var lastProject = projects[projects.Count - 1];
        lastProject.Project.SignificantDate = new DateOnly(2025, 7, 1);

        var inRangeProjects = projects
            .Where(p => p.Project.SignificantDate >= startDate && p.Project.SignificantDate <= endDate)
            .ToList()
            .BuildMock();

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .Where(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .GenerateQuery()
            .Returns(inRangeProjects);

        // Act
        var result = await handler.Handle(new ListProjectsByMonthsQuery(startDate, endDate, ProjectState.Active, ProjectType.Conversion), default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value?.Count);
    }
}
