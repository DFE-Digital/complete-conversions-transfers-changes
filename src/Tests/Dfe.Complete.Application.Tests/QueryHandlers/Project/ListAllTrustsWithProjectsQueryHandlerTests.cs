using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Microsoft.Extensions.Logging;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ProjectEntity = Dfe.Complete.Domain.Entities.Project;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListAllTrustsWithProjectsQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(typeof(OmitCircularReferenceCustomization))]
        public async Task Handle_ShouldReturnPagedTrusts_WhenProjectsExist(
            [Frozen] IProjectReadRepository repo,
            [Frozen] ITrustsV4Client trustsClient,
            [Frozen] ILogger<ListAllTrustsWithProjectsQueryHandler> logger,
            IFixture fixture)
        {
            // Arrange
            // Create projects: two non-MAT under UKPRN 100, one under 200; one MAT under T1
            var projects = new List<ProjectEntity>
            {
                new ProjectEntity { IncomingTrustUkprn = new Ukprn(100), Type = ProjectType.Conversion },
                new ProjectEntity { IncomingTrustUkprn = new Ukprn(100), Type = ProjectType.Transfer },
                new ProjectEntity { IncomingTrustUkprn = new Ukprn(200), Type = ProjectType.Conversion },
                new ProjectEntity { IncomingTrustUkprn = new Ukprn(200), NewTrustReferenceNumber = "T1", NewTrustName = "Trust One", Type = ProjectType.Transfer }
            }.AsQueryable().BuildMock();

            repo.Projects.Returns(projects);

            // Mock API return for non-MAT UKPRNs
            var dtos = new System.Collections.ObjectModel.ObservableCollection<TrustDto>
            {
                new TrustDto { Ukprn = "100", Name = "Alpha Trust", ReferenceNumber = "100" },
                new TrustDto { Ukprn = "200", Name = "Beta Trust", ReferenceNumber = "200" }
            };
            trustsClient.GetByUkprnsAllAsync(Arg.Is<IEnumerable<string>>(u => u.SequenceEqual(new[] { "100", "200" })), Arg.Any<CancellationToken>())
                .Returns(dtos);

            var handler = new ListAllTrustsWithProjectsQueryHandler(repo, trustsClient, logger);
            var query = new ListAllTrustsWithProjectsQuery { Page = 0, Count = 10 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var list = result.Value;
            // Expect 3 distinct trusts: 100,200,T1 => 3
            Assert.Equal(3, list!.Count);

            // Verify counts and names
            var first = list.Single(r => r.Identifier == "100");
            Assert.Equal("Alpha Trust", first.TrustName);
            Assert.Equal(1, first.ConversionCount);
            Assert.Equal(1, first.TransfersCount);

            var second = list.Single(r => r.Identifier == "200");
            Assert.Equal("Beta Trust", second.TrustName);
            Assert.Equal(1, second.ConversionCount);
            Assert.Equal(0, second.TransfersCount);

            var third = list.Single(r => r.Identifier == "T1");
            Assert.Equal("Trust One", third.TrustName);
            Assert.Equal(0, third.ConversionCount);
            Assert.Equal(1, third.TransfersCount);
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ShouldReturnEmpty_WhenNoProjects(
            [Frozen] IProjectReadRepository repo,
            [Frozen] ITrustsV4Client trustsClient,
            [Frozen] ILogger<ListAllTrustsWithProjectsQueryHandler> logger)
        {
            // Arrange
            var empty = new List<ProjectEntity>().AsQueryable().BuildMock();
            repo.Projects.Returns(empty);

            var handler = new ListAllTrustsWithProjectsQueryHandler(repo, trustsClient, logger);
            var query = new ListAllTrustsWithProjectsQuery { Page = 0, Count = 5 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
            Assert.Equal(0, result.ItemCount);
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ShouldCatchException_WhenApiFails(
            [Frozen] IProjectReadRepository repo,
            [Frozen] ITrustsV4Client trustsClient,
            [Frozen] ILogger<ListAllTrustsWithProjectsQueryHandler> logger)
        {
            // Arrange
            var projects = new List<ProjectEntity>
            {
                new ProjectEntity { IncomingTrustUkprn = new Ukprn(111), Type = ProjectType.Conversion }
            }.AsQueryable().BuildMock();
            repo.Projects.Returns(projects);

            trustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
                .Throws(new Exception("API error"));

            var handler = new ListAllTrustsWithProjectsQueryHandler(repo, trustsClient, logger);
            var query = new ListAllTrustsWithProjectsQuery { Page = 0, Count = 5 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("API error", result.Error);
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ShouldFilterOutInactiveProjects(
            [Frozen] IProjectReadRepository repo,
            [Frozen] ITrustsV4Client trustsClient,
            [Frozen] ILogger<ListAllTrustsWithProjectsQueryHandler> logger)
        {
            // Arrange: one active and one inactive project
            var projects = new List<ProjectEntity>
            {
                new ProjectEntity { State = ProjectState.Active, IncomingTrustUkprn = new Ukprn(300), Type = ProjectType.Conversion },
                new ProjectEntity { State = ProjectState.Completed, IncomingTrustUkprn = new Ukprn(300), Type = ProjectType.Transfer }
            }.AsQueryable().BuildMock();
            repo.Projects.Returns(projects);

            // API should only be called for active project
            var dtos = new System.Collections.ObjectModel.ObservableCollection<TrustDto>
            {
                new TrustDto { Ukprn = "300", Name = "Gamma Trust", ReferenceNumber = "300" }
            };
            trustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(dtos));

            var handler = new ListAllTrustsWithProjectsQueryHandler(repo, trustsClient, logger);
            var query = new ListAllTrustsWithProjectsQuery { Page = 0, Count = 5 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert: only one trust returned, filtering out inactive
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal("Gamma Trust", result.Value!.First().TrustName);
        }

        [Theory]
        [CustomAutoData]
        public async Task Handle_ShouldReturnOnlyMatTrusts_WhenOnlyMatProjects(
            [Frozen] IProjectReadRepository repo,
            [Frozen] ITrustsV4Client trustsClient,
            [Frozen] ILogger<ListAllTrustsWithProjectsQueryHandler> logger)
        {
            // Arrange: two MAT projects
            var projects = new List<ProjectEntity>
            {
                new ProjectEntity { State = ProjectState.Active, NewTrustReferenceNumber = "M1", IncomingTrustUkprn = "121", NewTrustName = "Mat One", Type = ProjectType.Conversion },
                new ProjectEntity { State = ProjectState.Active, NewTrustReferenceNumber = "M2", IncomingTrustUkprn = "121", NewTrustName = "Mat Two", Type = ProjectType.Transfer }
            }.AsQueryable().BuildMock();
            repo.Projects.Returns(projects);

            // API should not be called since no non-MAT
            await trustsClient.DidNotReceive().GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>());

            var handler = new ListAllTrustsWithProjectsQueryHandler(repo, trustsClient, logger);
            var query = new ListAllTrustsWithProjectsQuery { Page = 0, Count = 5 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert: two MAT trusts returned
            Assert.True(result.IsSuccess);
            var list = result.Value;
            Assert.Equal(2, list!.Count);
            Assert.Contains(list, r => r is { Identifier: "M1", TrustName: "Mat One" });
            Assert.Contains(list, r => r is { Identifier: "M2", TrustName: "Mat Two" });
        }
    }
}
