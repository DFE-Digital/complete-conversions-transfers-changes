using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListAllMATsQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnPaginatedMATList_WhenMATProjectsFound(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            ListAllMATsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var totalProjects = 30;
            var pageSize = 10;

            var listAllProjectsQueryModels = fixture
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

            var query = new ListAllMATsQuery { Count = pageSize, Page = 0 };

            var mockProjects = listAllProjectsQueryModels.BuildMock();

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                .Returns(mockProjects);

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
        public async Task Handle_ShouldReturnOnlyFormAMatProjects(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            ListAllMATsQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var matProjects = fixture
                .Build<ListAllProjectsQueryModel>()
                .CreateMany(5)
                .Select((p, index) =>
                {
                    p.Project.NewTrustReferenceNumber = $"TR{index}";
                    p.Project.NewTrustName = $"Test MAT {index}";
                    p.Project.IncomingTrustUkprn = null;
                    return p;
                });

            var otherProjects = fixture
                .Build<ListAllProjectsQueryModel>()
                .CreateMany(5)
                .Select(p => {
                    p.Project.NewTrustReferenceNumber = null;
                    p.Project.NewTrustName = null;
                    p.Project.IncomingTrustUkprn = new Ukprn(2);
                    return p;
                });

            var allProjects = matProjects.Concat(otherProjects).ToList();

            var mockProjects = allProjects.BuildMock();

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                .Returns(mockProjects);

            var query = new ListAllMATsQuery { Count = 10, Page = 0 };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.All(result.Value, item => Assert.Contains("Test MAT", item.trustName));
            Assert.Equal(5, result.Value.Count); 
        }

        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            ListAllMATsQueryHandler handler)
        {
            // Arrange
            var query = new ListAllMATsQuery { Count = 10, Page = 0 };

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                .Throws(new Exception("Test failure"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Test failure", result.Error);
        }
    }
}
