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
using Wmhelp.XPath2;

namespace Dfe.Complete.Application.Tests.QueryHandlers.SearchProjects
{
    public class SearchProjectsQueryHandlerTests
    { 
        [Theory]
        [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
            public async Task Handle_ShouldReturnFilteredList_WhenSearchTermURNs(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        SearchProjectsQueryHandler handler,
        IFixture fixture)
        {
            // Arrange  
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(2).ToList();
            var searchTerm = listAllProjectsQueryModels.First().Project!.Urn.Value.ToString()[..Math.Min(6, listAllProjectsQueryModels.First().Project!.Urn.Value.ToString().Length)];

            var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    item.Project!,
                    item.Establishment
                )).ToList();
             
            var mock = listAllProjectsQueryModels.BuildMock();

            var query = new SearchProjectsQuery(searchTerm, [ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked])
            {
                Page = 0,
                Count = 20
            };
            mockListAllProjectsQueryService
                .ListAllProjects(new ProjectFilters(null, null, ProjectStatuses: query.ProjectStates), searchTerm)
                .Returns(mock);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expected.Count, result.Value?.Count);
            Assert.Equal(expected, result.Value);
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
            var query = new SearchProjectsQuery(searchTerm, [ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked])
            {
                Page = 0,
                Count = 20
            };
            mockListAllProjectsQueryService
                .ListAllProjects(new ProjectFilters(null, null, ProjectStatuses: query.ProjectStates), searchTerm)
                .Returns(mock); 

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
            var listAllProjectsQueryModels = fixture.Build<ListAllProjectsQueryModel>()
               .CreateMany(1)
               .ToList();
            var searchTerm = listAllProjectsQueryModels.First().Establishment!.EstablishmentNumber![..4];

            var expected = listAllProjectsQueryModels.Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                    item.Project!,
                    item.Establishment
                )).ToList();

            var query = new SearchProjectsQuery(searchTerm, [ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked])
            {
                Page = 0,
                Count = 20
            };

            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService
                .ListAllProjects(new ProjectFilters(null,null, ProjectStatuses: query.ProjectStates), searchTerm)
                .Returns(mock);

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
            var query = new SearchProjectsQuery(searchTerm!, [ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked])
            {
                Page = 0,
                Count = 20
            };

            mockListAllProjectsQueryService
                .ListAllProjects(new ProjectFilters(null, null, ProjectStatuses: query.ProjectStates), searchTerm)
                .Returns(mock);
             
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
        public async Task Handle_ShouldReturnException_WhenExceptionOccurs(
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
              
            var query = new SearchProjectsQuery(searchTerm!, [ProjectState.Active, ProjectState.Completed, ProjectState.DaoRevoked])
            {
                Page = 1,
                Count = 20
            };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value?.Count);
        }
    }
}
