using AutoFixture.Xunit2;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using Dfe.Complete.Domain.Interfaces.Repositories;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.AcademiesApi.Client.Contracts;
using NSubstitute.ExceptionExtensions;
using System.Collections.ObjectModel;
using MockQueryable;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class GetProjectGroupsQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnProjectGroups_WhenQueryIsValid(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            [Frozen] IEstablishmentsV4Client mockEstablishmentsClient,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var projectGroups = new List<ProjectGroup>
            {
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP001", TrustUkprn = new Ukprn(12345678) },
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP002", TrustUkprn = new Ukprn(87654321) }
            };

            var projects = new List<Dfe.Complete.Domain.Entities.Project>
            {
                new() { Id = new ProjectId(Guid.NewGuid()), GroupId = projectGroups[0].Id, Urn = new Urn(123456) },
                new() { Id = new ProjectId(Guid.NewGuid()), GroupId = projectGroups[0].Id, Urn = new Urn(123457) },
                new() { Id = new ProjectId(Guid.NewGuid()), GroupId = projectGroups[1].Id, Urn = new Urn(123458) }
            };

            var trusts = new ObservableCollection<TrustDto>
            {
                new() { Ukprn = "12345678", Name = "Trust One" },
                new() { Ukprn = "87654321", Name = "Trust Two" }
            };

            var establishments = new ObservableCollection<EstablishmentDto>
            {
                new() { Urn = "123456", Name = "School One" },
                new() { Urn = "123457", Name = "School Two" },
                new() { Urn = "123458", Name = "School Three" }
            };

            mockProjectGroupRepository.Query().Returns(projectGroups.BuildMock());
            mockProjectRepository.Query().Returns(projects.BuildMock());
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(trusts));
            mockEstablishmentsClient.GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(establishments));

            // Act
            var result = await handler.Handle(query with { Page = 0, Count = 100 }, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
            
            var firstGroup = result.Value[0];
            Assert.Equal(projectGroups[1].Id.Value.ToString(), firstGroup.GroupId);
            Assert.Equal("Trust Two", firstGroup.GroupName);
            Assert.Equal("GROUP002", firstGroup.GroupIdentifier);
            Assert.Equal("87654321", firstGroup.TrustUkprn);
            Assert.Contains("School Three", firstGroup.IncludedEstablishments);
            
            var secondGroup = result.Value[1];
            Assert.Equal(projectGroups[0].Id.Value.ToString(), secondGroup.GroupId);
            Assert.Equal("Trust One", secondGroup.GroupName);
            Assert.Equal("GROUP001", secondGroup.GroupIdentifier);
            Assert.Equal("12345678", secondGroup.TrustUkprn);
            Assert.Contains("School One", secondGroup.IncludedEstablishments);
            Assert.Contains("School Two", secondGroup.IncludedEstablishments);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProjectGroupsExist(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            [Frozen] IEstablishmentsV4Client mockEstablishmentsClient,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var emptyProjectGroups = new List<ProjectGroup>();
            var emptyProjects = new List<Dfe.Complete.Domain.Entities.Project>();

            mockProjectGroupRepository.Query().Returns(emptyProjectGroups.BuildMock());
            mockProjectRepository.Query().Returns(emptyProjects.BuildMock());
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ObservableCollection<TrustDto>()));
            mockEstablishmentsClient.GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ObservableCollection<EstablishmentDto>()));

            // Act
            var result = await handler.Handle(query with { Page = 0, Count = 100 }, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldHandleProjectGroupsWithoutProjects(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            [Frozen] IEstablishmentsV4Client mockEstablishmentsClient,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var projectGroups = new List<ProjectGroup>
            {
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP001", TrustUkprn = new Ukprn(12345678) }
            };

            var emptyProjects = new List<Dfe.Complete.Domain.Entities.Project>();
            var trusts = new ObservableCollection<TrustDto>
            {
                new() { Ukprn = "12345678", Name = "Trust One" }
            };

            mockProjectGroupRepository.Query().Returns(projectGroups.BuildMock());
            mockProjectRepository.Query().Returns(emptyProjects.BuildMock());
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(trusts));
            mockEstablishmentsClient.GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ObservableCollection<EstablishmentDto>()));

            // Act
            var result = await handler.Handle(query with { Page = 0, Count = 100 }, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
            
            var group = result.Value[0];
            Assert.Equal(projectGroups[0].Id.Value.ToString(), group.GroupId);
            Assert.Equal("Trust One", group.GroupName);
            Assert.Equal("GROUP001", group.GroupIdentifier);
            Assert.Equal("12345678", group.TrustUkprn);
            Assert.Equal("", group.IncludedEstablishments);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFail_WhenProjectGroupRepositoryThrowsException(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            mockProjectGroupRepository.Query().Throws(new Exception("Database connection failed"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Database connection failed", result.Error);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFail_WhenProjectRepositoryThrowsException(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var projectGroups = new List<ProjectGroup>
            {
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP001", TrustUkprn = new Ukprn(12345678) }
            };

            mockProjectGroupRepository.Query().Returns(projectGroups.BuildMock());
            mockProjectRepository.Query().Throws(new Exception("Project repository failed"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Project repository failed", result.Error);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFail_WhenTrustsClientThrowsException(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var projectGroups = new List<ProjectGroup>
            {
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP001", TrustUkprn = new Ukprn(12345678) }
            };

            var projects = new List<Dfe.Complete.Domain.Entities.Project>
            {
                new() { Id = new ProjectId(Guid.NewGuid()), GroupId = projectGroups[0].Id, Urn = new Urn(123456) }
            };

            mockProjectGroupRepository.Query().Returns(projectGroups.BuildMock());
            mockProjectRepository.Query().Returns(projects.BuildMock());
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Throws(new Exception("Trusts API failed"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Trusts API failed", result.Error);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldFail_WhenEstablishmentsClientThrowsException(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            [Frozen] IEstablishmentsV4Client mockEstablishmentsClient,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var projectGroups = new List<ProjectGroup>
            {
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP001", TrustUkprn = new Ukprn(12345678) }
            };

            var projects = new List<Dfe.Complete.Domain.Entities.Project>
            {
                new() { Id = new ProjectId(Guid.NewGuid()), GroupId = projectGroups[0].Id, Urn = new Urn(123456) }
            };

            var trusts = new ObservableCollection<TrustDto>
            {
                new() { Ukprn = "12345678", Name = "Trust One" }
            };

            mockProjectGroupRepository.Query().Returns(projectGroups.BuildMock());
            mockProjectRepository.Query().Returns(projects.BuildMock());
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(trusts));
            mockEstablishmentsClient.GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Throws(new Exception("Establishments API failed"));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Establishments API failed", result.Error);
        }

        [Theory]
        [CustomAutoData(typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldOrderProjectGroupsByGroupIdentifierDescending(
            [Frozen] ICompleteRepository<ProjectGroup> mockProjectGroupRepository,
            [Frozen] ICompleteRepository<Dfe.Complete.Domain.Entities.Project> mockProjectRepository,
            [Frozen] ITrustsV4Client mockTrustsClient,
            [Frozen] IEstablishmentsV4Client mockEstablishmentsClient,
            GetProjectGroupsQueryHandler handler,
            ListProjectGroupsQuery query)
        {
            // Arrange
            var projectGroups = new List<ProjectGroup>
            {
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP001", TrustUkprn = new Ukprn(12345678) },
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP003", TrustUkprn = new Ukprn(87654321) },
                new() { Id = new ProjectGroupId(Guid.NewGuid()), GroupIdentifier = "GROUP002", TrustUkprn = new Ukprn(11111111) }
            };

            var projects = new List<Dfe.Complete.Domain.Entities.Project>();
            var trusts = new ObservableCollection<TrustDto>
            {
                new() { Ukprn = "12345678", Name = "Trust One" },
                new() { Ukprn = "87654321", Name = "Trust Two" },
                new() { Ukprn = "11111111", Name = "Trust Three" }
            };

            mockProjectGroupRepository.Query().Returns(projectGroups.BuildMock());
            mockProjectRepository.Query().Returns(projects.BuildMock());
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(trusts));
            mockEstablishmentsClient.GetByUrns2Async(Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(new ObservableCollection<EstablishmentDto>()));

            // Act
            var result = await handler.Handle(query with { Page = 0, Count = 100 }, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count);
            
            Assert.Equal("GROUP003", result.Value[0].GroupIdentifier);
            Assert.Equal("GROUP002", result.Value[1].GroupIdentifier);
            Assert.Equal("GROUP001", result.Value[2].GroupIdentifier);
        }
    }
} 