using System.Collections.ObjectModel;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListProjectsByMonth;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsByMonthHandlerTests
{
    [Theory]
    [CustomAutoData]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsByMonthQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";

        mockListAllProjectsQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Throws(new Exception(errorMessage));

        // Act
        var result = await handler.Handle(new ListProjectsByMonthQuery(1, 2025, ProjectState.Active, ProjectType.Conversion), default);

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
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        [Frozen] ITrustsV4Client mockTrustsClient,
        ListAllProjectsByMonthQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var expectedError = "Trust lookup error";
        var mockProjects = fixture.CreateMany<ListAllProjectsQueryModel>(3).BuildMock();
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>()).Returns(mockProjects);
        mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Throws(new Exception(expectedError));
        
        // Act
        var result = await handler.Handle(new ListProjectsByMonthQuery(1, 2025, ProjectState.Active, ProjectType.Conversion), default);
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Contains(expectedError, result.Error);
    }
    
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsByMonthQueryHandler handler,
        IFixture fixture)
    {
        //Arrange 
        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
    
        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
    
        mockListAllProjectsQueryService
            .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(listAllProjectsMock);
    
        //Act
        var handlerResult =
            await handler.Handle(new ListProjectsByMonthQuery(1,1, ProjectState.Active, ProjectType.Conversion) { Page = 10 }, default);
    
        //Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);
        
        mockListAllProjectsQueryService.Received(1).ListAllProjects(
            Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>());
    }

    [Theory]
    [CustomAutoData]
    public async Task Handle_ShouldReturnEmpty_WhenNoProjectsMatch(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsByMonthQueryHandler handler)
    {
        // Arrange
        var projects = Enumerable.Empty<ListAllProjectsQueryModel>().BuildMock();
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>()).Returns(projects);
        
        // Act
        var result = await handler.Handle(new ListProjectsByMonthQuery(1, 2025, ProjectState.Active, ProjectType.Conversion), default);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldCorrectlyPaginateResults(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        [Frozen] ITrustsV4Client mockTrustsClient,
        ListAllProjectsByMonthQueryHandler handler,
        IFixture fixture)
    {
        //Arrange
        var ukprn = fixture.Create<int>();
        var day = 1;
        var month = 1;
        var year = 2025;
        
        var listAllProjectsQueryModels = fixture.Build<ListAllProjectsQueryModel>()
            .CreateMany(50)
            .Select(p =>
            {
                p.Project.IncomingTrustUkprn = new Ukprn(ukprn);
                p.Project.SignificantDate = new DateOnly(year, month, day);
                return p;
            })
            .BuildMock();
        
        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>()).Returns(listAllProjectsMock);
        
        
        var trustDtos = fixture
            .Build<TrustDto>()
            .With(t => t.Ukprn, ukprn.ToString())
            .CreateMany(1)
            .OrderBy(t => t.Name);

        mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ObservableCollection<TrustDto>(trustDtos)));
        
        
        // Act
        var result = await handler.Handle(new ListProjectsByMonthQuery(month, year, ProjectState.Active, ProjectType.Conversion, Page: 2, Count: 10), default);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value?.Count);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectValues_WhenRequestReturnsResults(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        [Frozen] ITrustsV4Client mockTrustsClient,
        ListAllProjectsByMonthQueryHandler handler,
        IFixture fixture)
        {
            //Arrange
            var ukprn = fixture.Create<int>();
            var day = 1;
            var month = 1;
            var year = 2025;
            
            var mockProjects = fixture.Build<ListAllProjectsQueryModel>()
                .CreateMany(1)
                .Select(p =>
                {
                    p.Project.IncomingTrustUkprn = new Ukprn(ukprn);
                    p.Project.SignificantDate = new DateOnly(year, month, day);
                    return p;
                })
                .BuildMock();

            mockListAllProjectsQueryService
                .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
                .Returns(mockProjects);

            var trustDtos = fixture
                .Build<TrustDto>()
                .With(t => t.Ukprn, ukprn.ToString())
                .CreateMany(1)
                .OrderBy(t => t.Name);

            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ObservableCollection<TrustDto>(trustDtos)));
            
            //Act
            var handlerResult =
                await handler.Handle(new ListProjectsByMonthQuery(month, year, ProjectState.Active, ProjectType.Conversion) { Page = 0 }, default);

            //Assert
            Assert.NotNull(handlerResult.Value);
            Assert.True(handlerResult.IsSuccess);
            Assert.Equal(1, handlerResult.Value?.Count);
            
            mockListAllProjectsQueryService.Received(1).ListAllProjects(
                Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>());
            
            var project = handlerResult.Value!.First();
            var expectedProject = mockProjects.First();
            
            Assert.Equal(expectedProject.Establishment.Name, project.EstablishmentName);
            Assert.Equal(expectedProject.Project.Region.ToDisplayDescription(), project.Region);
            Assert.Equal(expectedProject.Project.LocalAuthority?.Name, project.LocalAuthority);
            Assert.Equal(expectedProject.Project.Id, project.ProjectId);
            Assert.Equal(expectedProject.Project.Urn, project.Urn);
            Assert.Equal(expectedProject.Project.Type, project.ProjectType);
            
            var expectedIncomingTrust = expectedProject.Project.FormAMat 
                ? expectedProject.Project.NewTrustName 
                : trustDtos.FirstOrDefault(t => t.Ukprn == expectedProject.Project.IncomingTrustUkprn?.Value.ToString())?.Name;
            
            Assert.Equal(expectedIncomingTrust, project.IncomingTrust);
            
            var expectedOutgoingTrust = trustDtos.FirstOrDefault(t => t.Ukprn == expectedProject.Project.OutgoingTrustUkprn?.Value.ToString())?.Name;
            Assert.Equal(expectedOutgoingTrust, project.OutgoingTrust);
            
            var expectedAuthorityToProceed = expectedProject.Project.AllConditionsMet.HasValue && expectedProject.Project.AllConditionsMet.Value ? "Yes" : "Not yet";
            Assert.Equal(expectedAuthorityToProceed, project.AllConditionsMet);
            
            var confirmedDate = expectedProject.Project.SignificantDate?.ToString("MMM yyyy");
            var originalDate = expectedProject.Project.SignificantDateHistories.Any() 
                ? expectedProject.Project.SignificantDateHistories.FirstOrDefault()?.PreviousDate?.ToString("MMM yyyy")
                : null;
            
            var expectedConfirmedAndOriginalDate = string.IsNullOrEmpty(originalDate) ? confirmedDate : $"{confirmedDate} ({originalDate})";
            Assert.Equal(expectedConfirmedAndOriginalDate, project.ConfirmedAndOriginalDate);
        }
}