using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using DfE.CoreLibs.Caching.Interfaces;
using Dfe.Complete.Application.Common.Models;
using DfE.CoreLibs.Caching.Helpers;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Queries.CountAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Dfe.Complete.Tests.Common.Customizations.Queries;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListAllProjectsQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
            [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
            ListAllProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
            
            var expected = listAllProjectsQueryModels.Select(item => new ListAllProjectsResultModel(
                item.Establishment.Name,
                item.Project.Id,
                item.Project.Urn,
                item.Project.SignificantDate,
                item.Project.State,
                item.Project.Type,
                item.Project.IncomingTrustUkprn == null,
                item.Project.AssignedTo != null
                    ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                    : null)).Take(20).ToList();

            var query = new ListAllProjectsQuery(null, null);

            var mock = listAllProjectsQueryModels.BuildMock();

            mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
                .Returns(mock);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Count, result.Value?.Count);

            for (int i = 0; i < result.Value!.Count; i++)
            {
                Assert.Equivalent(expected[i], result.Value![i]);
            }
        }
        
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnCorrectList_ForOtherPages(
            [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
            ListAllProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
            
            var expected = listAllProjectsQueryModels.Select(item => new ListAllProjectsResultModel(
                item.Establishment.Name,
                item.Project.Id,
                item.Project.Urn,
                item.Project.SignificantDate,
                item.Project.State,
                item.Project.Type,
                item.Project.IncomingTrustUkprn == null,
                item.Project.AssignedTo != null
                    ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                    : null)).Skip(20).Take(20).ToList();

            var query = new ListAllProjectsQuery(null, null, 1);


            var mock = listAllProjectsQueryModels.BuildMock();

            mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
                .Returns(mock);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Count, result.Value?.Count);

            for (int i = 0; i < result.Value!.Count; i++)
            {
                Assert.Equivalent(expected[i], result.Value![i]);
            }
        }
        
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
            [Frozen] IListAllProjectsQueryService mockEstablishmentQueryService,
            ListAllProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
            
            var query = new ListAllProjectsQuery(null, null, 10);

            var mock = listAllProjectsQueryModels.BuildMock();

            mockEstablishmentQueryService.ListAllProjects(query.ProjectStatus, query.Type)
                .Returns(mock);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Value?.Count);
        }
    }
}