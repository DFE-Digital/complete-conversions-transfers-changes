using AutoFixture;
using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListEstablishmentsInMatQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnMatchingProjects_WhenProjectsWithReferenceNumberExist(
            [Frozen] IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService,
            ListEstablishmentsInMatQueryHandler handler,
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
            
            var mockProjects = matchingProjects.BuildMock();

            listAllProjectsByFilterQueryService
                .ListAllProjectsByFilter(ProjectState.Active, null, newTrustReferenceNumber: referenceNumber)
                .Returns(mockProjects);

            var query = new ListEstablishmentsInMatQuery(referenceNumber);

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
            [Frozen] IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService,
            ListEstablishmentsInMatQueryHandler handler)
        {
            // Arrange
            var referenceNumber = "TR999";
            var emptyList = new List<ListAllProjectsQueryModel>().BuildMock();

            listAllProjectsByFilterQueryService
                .ListAllProjectsByFilter(ProjectState.Active, null, newTrustReferenceNumber: referenceNumber)
                .Returns(emptyList);

            var query = new ListEstablishmentsInMatQuery(referenceNumber);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No projects found", result.Error);
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown(
            [Frozen] IListAllProjectsByFilterQueryService listAllProjectsByFilterQueryService,
            ListEstablishmentsInMatQueryHandler handler)
        {
            // Arrange
            var expectedError = "Test failure";
            var query = new ListEstablishmentsInMatQuery("TR123");

            listAllProjectsByFilterQueryService
                .ListAllProjectsByFilter(ProjectState.Active, null,  newTrustReferenceNumber: "TR123")
                .Throws(new Exception(expectedError));

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedError, result.Error);
        }
    }
}
