using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.AcademiesApi.Client.Contracts;
using MockQueryable;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectGroupDetailsQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnDetails_WhenQueryIsValid(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            [Frozen] IEstablishmentsV4Client mockEstablishmentsClient,
            GetProjectGroupDetailsQueryHandler handler,
            GetProjectGroupDetailsQuery query)
        {
            // Arrange
            var groupId = new ProjectGroupId(Guid.NewGuid());
            query = new GetProjectGroupDetailsQuery(groupId);

            var projectGroup = new ProjectGroup
            {
                Id = groupId,
                GroupIdentifier = "GROUP-001",
                TrustUkprn = new Ukprn(12345678)
            };

            var localAuthority = new LocalAuthority { Id = new LocalAuthorityId(Guid.NewGuid()), Name = "LA One" };

            var projects = new List<Dfe.Complete.Domain.Entities.Project>
            {
                new()
                {
                    Id = new ProjectId(Guid.NewGuid()),
                    GroupId = groupId,
                    Urn = new Urn(100001),
                    Type = Dfe.Complete.Domain.Enums.ProjectType.Conversion,
                    LocalAuthority = localAuthority,
                    Region = Dfe.Complete.Domain.Enums.Region.London
                },
                new()
                {
                    Id = new ProjectId(Guid.NewGuid()),
                    GroupId = groupId,
                    Urn = new Urn(100002),
                    Type = Dfe.Complete.Domain.Enums.ProjectType.Transfer,
                    LocalAuthority = localAuthority,
                    Region = Dfe.Complete.Domain.Enums.Region.NorthWest
                }
            };

            var trusts = new TrustDto { Ukprn = projectGroup.TrustUkprn.Value.ToString(), Name = "Trust One", ReferenceNumber = "TR001" };

            var establishments = new System.Collections.ObjectModel.ObservableCollection<EstablishmentDto>
            {
                new() { Urn = "100001", Name = "School One" },
                new() { Urn = "100002", Name = "School Two" }
            };

            mockProjectGroupRepository.Query().Returns(new List<ProjectGroup> { projectGroup }.BuildMock());
            mockTrustsClient.GetTrustByUkprn2Async(projectGroup.TrustUkprn.Value.ToString(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(trusts));
            mockProjectRepository.Query().Returns(projects.BuildMock());
            mockEstablishmentsClient.GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(establishments));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(projectGroup.Id.Value.ToString(), result.Value.GroupId);
            Assert.Equal("Trust One", result.Value.TrustName);
            Assert.Equal("TR001", result.Value.TrustReference);
            Assert.Equal("GROUP-001", result.Value.GroupReference);
            Assert.Equal(2, result.Value.ProjectDetails.Count());
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFail_WhenProjectGroupNotFound(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            GetProjectGroupDetailsQueryHandler handler,
            GetProjectGroupDetailsQuery query)
        {
            // Arrange
            mockProjectGroupRepository.Query().Returns(new List<ProjectGroup>().BuildMock());

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFail_WhenTrustsClientThrows(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            GetProjectGroupDetailsQueryHandler handler,
            GetProjectGroupDetailsQuery query)
        {
            // Arrange
            var group = new ProjectGroup { Id = query.ProjectGroupId, TrustUkprn = new Ukprn(12345678), GroupIdentifier = "GROUP" };
            mockProjectGroupRepository.Query().Returns(new List<ProjectGroup> { group }.BuildMock());
            mockTrustsClient.GetTrustByUkprn2Async(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Throws(new Exception("Trusts API failed"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Trusts API failed", result.Error);
        }
    }
}