using System.Collections.ObjectModel;
using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllMATsQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnPaginatedMATList_WhenMATProjectsFound(
        [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
        [Frozen] IEstablishmentsV4Client establishmentsClient,
        ListAllMaTsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var totalProjects = 30;
        var pageSize = 10;

        var projects = fixture
            .Build<ListAllProjectsQueryModel>()
            .CreateMany(totalProjects)
            .Select((p, index) =>
            {
                p.Project.NewTrustReferenceNumber = $"TR{index}";
                p.Project.NewTrustName = $"Test Trust {index}";
                p.Project.IncomingTrustUkprn = null;
                return p;
            })
            .ToList();

        var projectsResult = projects.AsQueryable().BuildMock();

        listAllProjectsQueryService
            .ListAllProjects(ProjectState.Active, null, isFormAMat: true)
            .Returns(projectsResult);

        var urns = projects.Select(p => p.Project.Urn.Value.ToString()).Distinct().ToList();
        var establishmentDtos = urns
            .Select(u => new EstablishmentDto
            {
                Urn = u.ToString(),
                Name = $"School {u}"
            });

        establishmentsClient
            .GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(new ObservableCollection<EstablishmentDto>(establishmentDtos));

        var query = new ListAllMaTsQuery(ProjectState.Active) { Count = pageSize, Page = 0 };

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(pageSize, result.Value.Count);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnOnlyFormAMatProjectsGroupedByTrust(
        [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
        [Frozen] IEstablishmentsV4Client establishmentsClient,
        ListAllMaTsQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var trustKey = "TR999";
        var trustName = "Test MAT";
        var matProjects = fixture
            .Build<ListAllProjectsQueryModel>()
            .CreateMany(5)
            .Select((p, index) =>
            {
                p.Project.NewTrustReferenceNumber = trustKey;
                p.Project.NewTrustName = trustName;
                p.Project.Urn = new Urn(index + 100);
                return p;
            })
            .ToList();
    
        var dbProjects = matProjects.AsQueryable().BuildMock();
    
        listAllProjectsQueryService
            .ListAllProjects(ProjectState.Active, null, isFormAMat: true)
            .Returns(dbProjects);

        var establishments = matProjects.Select(p => new EstablishmentDto
        {
            Urn = p.Project.Urn.Value.ToString(),
            Name = $"School {p.Project.Urn.Value}"
        }).ToList();
    
        establishmentsClient
            .GetByUrns2Async(Arg.Any<List<int>>(), Arg.Any<CancellationToken>())
            .Returns(new ObservableCollection<EstablishmentDto>(establishments));

    
        var query = new ListAllMaTsQuery(ProjectState.Active) { Count = 10, Page = 0 };
    
        // Act
        var result = await handler.Handle(query, default);
        var expectedModel = result.Value[0];

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(trustKey, expectedModel.identifier);
        Assert.Equal(trustName, expectedModel.trustName);
        Assert.Equal(5, expectedModel.projectModels.Count());
    }
    
    [Theory]
    [CustomAutoData]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown(
        [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
        ListAllMaTsQueryHandler handler)
    {
        // Arrange
        var query = new ListAllMaTsQuery(ProjectState.Active) { Count = 10, Page = 0 };
    
        listAllProjectsQueryService
            .ListAllProjects(ProjectState.Active, null, isFormAMat: true)
            .Throws(new Exception("Exception"));
    
        // Act
        var result = await handler.Handle(query, default);
    
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Exception", result.Error);
    }
    
    
}
