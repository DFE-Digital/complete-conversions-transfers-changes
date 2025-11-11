using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.ObjectModel;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project
{
    public class ListAllProjectsHandoverQueryHandlerTests
    {
        [Theory]
        [CustomAutoData(
           typeof(OmitCircularReferenceCustomization),
           typeof(ListAllProjectsQueryModelCustomization),
           typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnPaginatedResult_WhenProjectsExist(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            ListAllProjectsHandoverQueryHandler handler,
            [Frozen] ITrustsV4Client mockTrustsClient,
            IFixture fixture)
        {
            // Arrange
            var ukprn = fixture.Create<int>();
            var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(10).ToList();

            var expected = listAllProjectsQueryModels.Select(item =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(item.Project,
                        item.Establishment)).ToList();
            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService.ListAllProjects(
                Arg.Is<ProjectFilters>(f =>
                    f.ProjectStatus == ProjectState.Inactive))
                .Returns(mock);
            var trustDtos = fixture
               .Build<TrustDto>()
               .With(t => t.Ukprn, ukprn.ToString())
               .CreateMany(1)
               .OrderBy(t => t.Name);
            mockTrustsClient.GetByUkprnsAllAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ObservableCollection<TrustDto>(trustDtos)));

            var query = new ListAllProjectsHandoverQuery(ProjectState.Inactive);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, result.ItemCount);
            Assert.NotNull(result.Value);
            foreach (var item in result.Value)
            {
                Assert.NotNull(item.NewTrustName);
            }
        }
        [Theory]
        [CustomAutoData(
           typeof(OmitCircularReferenceCustomization),
           typeof(ListAllProjectsQueryModelCustomization),
           typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnEmptyPaginatedResult_WhenNoTrustUkprnsExist(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            ListAllProjectsHandoverQueryHandler handler,
            IFixture fixture)
        {
            // Arrange
            var ukprn = fixture.Create<int>();
            var listAllProjectsQueryModels = fixture.Build<ListAllProjectsQueryModel>()
            .CreateMany(5)
            .Select(p =>
            {
                p.Project.State = ProjectState.Inactive;
                p.Project.NewTrustName = null;
                p.Project.IncomingTrustUkprn = null;
                p.Project.OutgoingTrustUkprn = null;
                return p;
            })
            .BuildMock();
            var expected = listAllProjectsQueryModels.Select(item =>
                    ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(item.Project,
                        item.Establishment)).ToList();
            var mock = listAllProjectsQueryModels.BuildMock();

            mockListAllProjectsQueryService.ListAllProjects(
                Arg.Is<ProjectFilters>(f =>
                    f.ProjectStatus == ProjectState.Inactive))
                .Returns(mock);

            var query = new ListAllProjectsHandoverQuery(ProjectState.Inactive);

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.ItemCount);
            Assert.NotNull(result.Value);
            foreach (var item in result.Value)
            {
                Assert.Null(item.NewTrustName);
            }
        }

        [Theory]
        [CustomAutoData(
           typeof(OmitCircularReferenceCustomization),
           typeof(ListAllProjectsQueryModelCustomization),
           typeof(DateOnlyCustomization))]
        public async Task Handle_ShouldReturnFailure_WhenExceptionThrown(
            [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
            ListAllProjectsHandoverQueryHandler handler)
        {
            // Arrange  
            mockListAllProjectsQueryService.ListAllProjects(
                Arg.Is<ProjectFilters>(f =>
                    f.ProjectStatus == ProjectState.Inactive &&
                    f.Team == ProjectTeam.RegionalCaseWorkerServices))
               .Throws(new Exception("Something went wrong"));

            var query = new ListAllProjectsHandoverQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
        }
    }

}
