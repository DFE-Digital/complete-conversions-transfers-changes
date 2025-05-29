using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using MockQueryable;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListAllProjectsInTrustTests
    {
        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(ListAllProjectsQueryModelCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnCorrectResult_WhenProjectsFound(
            [Frozen] IListAllProjectsQueryService listAllProjectsQueryService,
            [Frozen] ITrustsV4Client trustsClient,
            ListAllProjectsInTrustQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var expectedTrusts = 20;
            var expectedProjects = 20;
            var ukprn = 2;

            var trustDto = fixture
                .Build<TrustDto>().With(t => t.Ukprn, ukprn.ToString())
                .Create();

            trustsClient.GetTrustByUkprn2Async(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(trustDto));

            var listAllProjectsQueryModels = fixture
                .Build<ListAllProjectsQueryModel>()
                .CreateMany(expectedProjects)
                .ToList();

            var random = new Random();

            foreach (var model in listAllProjectsQueryModels)
            {
                var project = model.Project;
                bool isMatProject = random.NextDouble() < 0.5; // 50% chance

                project.IncomingTrustUkprn = new Ukprn(ukprn);
                project.Type = isMatProject ? ProjectType.Conversion : ProjectType.Transfer;

                if (isMatProject)
                {
                    project.NewTrustReferenceNumber = "TR0001";
                    project.NewTrustName = "Test Trust";
                }
            }

            var query = new ListAllProjectsInTrustQuery("", false) { Count = expectedTrusts };

            var mockProjects = listAllProjectsQueryModels.BuildMock();

            listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null, newTrustReferenceNumber: null, incomingTrustUkprn: ukprn.ToString())
                .Returns(mockProjects);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Value.TrustName, trustDto.Name);
            Assert.Equal(20, result.Value.Projects.Count());
        }

        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ExceptionIsCaught_WhenTrustClientFails(
            [Frozen] ITrustsV4Client trustsClient,
            ListAllProjectsInTrustQueryHandler handler)
        {
            // Arrange
            var errorMessage = "This is an error";

            var query = new ListAllProjectsInTrustQuery("", false) { Count = 20 };

            trustsClient.GetTrustByUkprn2Async(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Throws(new Exception(errorMessage));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.Error);
        }

        [Theory]
        [CustomAutoData(
            typeof(OmitCircularReferenceCustomization),
            typeof(DateOnlyCustomization))]
        public async Task Handle_ExceptionIsCaught_WhenTrustNotFound(
            [Frozen] ITrustsV4Client trustsClient,
            ListAllProjectsInTrustQueryHandler handler)
        {
            // Arrange
            var query = new ListAllProjectsInTrustQuery("1212123", false) { Count = 20 };

            trustsClient.GetTrustByUkprn2Async(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<TrustDto>(null!));

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Trust with UKPRN 1212123 not found.", result.Error);
        }
    }
}