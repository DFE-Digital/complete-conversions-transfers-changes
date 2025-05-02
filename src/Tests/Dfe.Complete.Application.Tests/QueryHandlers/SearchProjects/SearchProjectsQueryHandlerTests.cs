using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models; 
using Dfe.Complete.Application.Projects.Queries.SearchProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute; 

namespace Dfe.Complete.Application.Tests.QueryHandlers.SearchProjects
{
    public class SearchProjectsQueryHandlerTests
    { 
        [Theory]
        [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFilteredList_WhenSearchTermURN(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            SearchProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange 
            var pageCount = 100;
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(1).ToList();
            var searchTerm = listAllProjectsQueryModels.First().Project!.Urn.Value.ToString()[..Math.Min(6, listAllProjectsQueryModels.First().Project!.Urn.Value.ToString().Length)];

            var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    item.Project!,
                    item.Establishment
                )).ToList();

            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService
                .SearchProjects(ProjectState.Active, searchTerm, pageCount)
                .Returns(mock);

            var query = new SearchProjectsQuery(ProjectState.Active, searchTerm, 0, 10);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(listAllProjectsQueryModels.Count, result.Value?.Count);
        }

        [Theory]
        [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFilteredList_WhenSearchTermIsUKPRN(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            SearchProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange 
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(1).ToList();
            var searchTerm = listAllProjectsQueryModels.First().Project!.IncomingTrustUkprn!.Value.ToString()[..Math.Min(8, listAllProjectsQueryModels.First().Project!.IncomingTrustUkprn!.Value.ToString().Length)];

            var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    item.Project!,
                    item.Establishment
                )).ToList();

            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService
                .SearchProjects(ProjectState.Active, searchTerm, 100)
                .Returns(mock);

            var query = new SearchProjectsQuery(ProjectState.Active, searchTerm, 0, 10);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(listAllProjectsQueryModels.Count, result.Value?.Count);
        }

        [Theory]
        [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFilteredList_WhenSearchTermIsEstablishmentNumber(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            SearchProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange 
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(1).ToList();
            var searchTerm = listAllProjectsQueryModels.First().Establishment!.EstablishmentNumber![..4];

            var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    item.Project!,
                    item.Establishment
                )).ToList();

            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService
                .SearchProjects(ProjectState.Active, searchTerm, 100)
                .Returns(mock);

            var query = new SearchProjectsQuery(ProjectState.Active, searchTerm, 0, 10);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(listAllProjectsQueryModels.Count, result.Value?.Count);
        }

        [Theory]
        [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFilteredList_WhenSearchTermIsString(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            SearchProjectsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange 
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(1).ToList();
            var searchTerm = listAllProjectsQueryModels.First().Establishment!.Name;

            var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    item.Project!,
                    item.Establishment
                )).ToList();

            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService
                .SearchProjects(ProjectState.Active, searchTerm!, 100)
                .Returns(mock);

            var query = new SearchProjectsQuery(ProjectState.Active, searchTerm!, 0, 10);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(listAllProjectsQueryModels.Count, result.Value?.Count);
        }
    }
}
