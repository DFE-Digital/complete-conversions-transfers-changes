using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectLocalAuthoritiesQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectLocalAuthoritiesArrangementCustomization))]
    public async Task Handle_ShouldReturnListProjectsLocalAuthorities(
        ListAllProjectLocalAuthorities handler,
        IFixture fixture)
    {
        //Arrange
        var expectedLocalAuthorities = fixture.Create<List<ListAllProjectLocalAuthoritiesResultModel>>();
        
        //Act
        var query = new ListAllProjectLocalAuthoritiesQuery();

        var handlerResult = await handler.Handle(query, default);
        
        //Assert 
        Assert.NotNull(handlerResult);
        Assert.Equal(expectedLocalAuthorities.Count, handlerResult.ItemCount);

        for (var i = 0; i < handlerResult.ItemCount; i++)
        {
            Assert.Equivalent(expectedLocalAuthorities[i], handlerResult.Value![i]);
        }
    }
    
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectLocalAuthoritiesArrangementCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        [Frozen] ICompleteRepository<LocalAuthority> localAuthoritiesRepo,
        ListAllProjectLocalAuthorities handler,
        IFixture fixture)
    {
        //Act
        var query = new ListAllProjectLocalAuthoritiesQuery { Page = 10 };

        var handlerResult = await handler.Handle(query, default);
        
        //Assert 
       
        // Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);
    }
}