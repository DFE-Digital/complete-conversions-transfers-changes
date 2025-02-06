using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class CountAllProjectsForRegionQueryHandlerTest
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectCount(
        [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
        CountAllProjectsForRegionQueryHandler handler,
        CountAllProjectsForRegionQuery query,
        List<ListAllProjectsQueryModel> listAllProjectsQueryModels)
    {
        // Arrange            
        var expected = listAllProjectsQueryModels.Count;

        listAllProjectsQueryModels.Take(expected).ToList().ForEach(p => p.Project.Region = query.Region);

        var mock = listAllProjectsQueryModels.BuildMock();

        mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Returns(mock);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnUnsuccessful_WhenAnErrorOccurs(
        [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
        CountAllProjectsForRegionQuery query,
        CountAllProjectsForRegionQueryHandler handler)
    {
        // Arrange
        var errorMessage = "This is a test";
        
        mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
            .Throws(new Exception(errorMessage));
    
        // Act
        var result = await handler.Handle(query, default);
    
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
}