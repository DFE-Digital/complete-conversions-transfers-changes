using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Linq.Expressions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsByRegionQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(ProjectsQueryBuilderCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnProjectsByRegion(
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        ListAllProjectsByRegionQueryHandler handler,
        IFixture fixture)
    {
        // Arrange
        var listAllProjectsQueryModels = fixture.CreateMany<Domain.Entities.Project>(50).ToList();

        var query = new ListAllProjectsQuery(null, null);

        var mock = listAllProjectsQueryModels.BuildMock();


        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .Where(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .GetProjects()
            .Returns(mock);

        var listAllProjectsByRegionQuery = new ListAllProjectsByRegionQuery(null, null);

        var result = await handler.Handle(listAllProjectsByRegionQuery, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Distinct(result.Value.Select(x => x.Region));
        Assert.Contains(result.Value, x => x.TransfersCount > 1 || x.ConversionsCount > 1);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(ProjectsQueryBuilderCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ThrowsException_Returns_FailureResult(
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        ListAllProjectsByRegionQueryHandler handler)
    {
        // Arrange
        const string errorMessage = "test error message";

        var query = new ListAllProjectsQuery(null, null);

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .Where(Arg.Any<Expression<Func<Domain.Entities.Project, bool>>>())
            .GetProjects()
            .Throws(new Exception(errorMessage));

        var listAllProjectsByRegionQuery = new ListAllProjectsByRegionQuery(null, null);

        var result = await handler.Handle(listAllProjectsByRegionQuery, CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(errorMessage, result.Error);
    }
}