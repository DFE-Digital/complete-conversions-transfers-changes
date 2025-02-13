using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Application.Projects.Queries.ListAllProjectsForLocalAuthority;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsForLAQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectsQueryModelCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture)
    {
        //Arrange 
        var localAuthorityCode = fixture.Create<string>();
        var expectedProjectsWithLaCodeCount = 10;

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();

        var takenProjects = listAllProjectsQueryModels
            .Take(expectedProjectsWithLaCodeCount)
            .ToList();

        takenProjects.ForEach(model => model.Establishment!.LocalAuthorityCode = localAuthorityCode);
        
        var expectedProjectIds = takenProjects.Select(x => x.Project.Id);
        
        var listAllProjectsMock = listAllProjectsQueryModels.BuildMock();
        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(listAllProjectsMock);

        //Act
        var handlerResult =
            await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(localAuthorityCode), default);

        Assert.NotNull(handlerResult.Value);
        Assert.Equal(expectedProjectsWithLaCodeCount, handlerResult.ItemCount);

        var actualProjectIds = handlerResult.Value.Select(x => x.ProjectId);
        Assert.All(expectedProjectIds, expectedId =>
        {
            Assert.Contains(expectedId, actualProjectIds);
        });
    }
}