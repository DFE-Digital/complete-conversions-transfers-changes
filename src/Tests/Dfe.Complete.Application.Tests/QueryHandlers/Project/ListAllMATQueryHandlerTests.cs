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
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListMatQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnMatchingProjects_WhenProjectsWithReferenceNumberExist(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            ListMatQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var referenceNumber = "TR123";
            var trustName = "Test MAT 123";
            
            var matchingProjects = fixture
                .Build<ListAllProjectsQueryModel>()
                .CreateMany(3)
                .Select(p => {
                    p.Project.NewTrustReferenceNumber = referenceNumber;
                    p.Project.NewTrustName = trustName;
                    p.Project.IncomingTrustUkprn = null;
                    return p;
                })
                .ToList();

            // Create projects with different reference numbers
            var otherProjects = fixture
                .Build<ListAllProjectsQueryModel>()
                .CreateMany(5)
                .Select((p, index) => {
                    p.Project.NewTrustReferenceNumber = $"TR{index + 200}";
                    p.Project.NewTrustName = $"Other MAT {index}";
                    p.Project.IncomingTrustUkprn = null;
                    return p;
                })
                .ToList();

            var allProjects = matchingProjects.Concat(otherProjects).ToList();
            var mockProjects = allProjects.BuildMock();

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                .Returns(mockProjects);

            var query = new ListMatQuery(referenceNumber);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(referenceNumber, result.Value.identifier);
            Assert.Equal(trustName, result.Value.trustName);
            Assert.Equal(matchingProjects.Count, result.Value.projectModels.Count());
            Assert.All(result.Value.projectModels, p => Assert.Equal(referenceNumber, p.Project.NewTrustReferenceNumber));
        }

        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFailure_WhenNoProjectsWithReferenceNumberExist(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            ListMatQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var searchReferenceNumber = "TR999";
            
            var projects = fixture
                .Build<ListAllProjectsQueryModel>()
                .CreateMany(5)
                .Select((p, index) => {
                    p.Project.NewTrustReferenceNumber = $"TR{index}";
                    p.Project.NewTrustName = $"Test MAT {index}";
                    p.Project.IncomingTrustUkprn = null;
                    return p;
                })
                .ToList();

            var mockProjects = projects.BuildMock();

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                .Returns(mockProjects);

            var query = new ListMatQuery(searchReferenceNumber);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No projects found", result.Error);
        }

        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            ListMatQueryHandler handler)
        {
            // Arrange
            var expectedError = "Test failure";
            var query = new ListMatQuery("TR123");

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                .Throws(new Exception(expectedError));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedError, result.Error);
        }
    }
}