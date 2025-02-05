using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsByRegionQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnProjectsByRegion(
        [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
        ListAllProjectsByRegionQueryHandler handler, 
        IFixture fixture)
    {
        
    }
}