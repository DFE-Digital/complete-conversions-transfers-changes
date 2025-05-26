using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectByLocalAuthoritiesQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
    typeof(OmitCircularReferenceCustomization),
    typeof(DateOnlyCustomization),
    typeof(ProjectCustomization))]
    public async Task Handle_ShouldReturnListProjectsLocalAuthorities(
    [Frozen] IListAllProjectsWithLAsQueryService mockListAllProjectsWithLAsQueryService,
    ListAllProjectByLocalAuthorities handler,
    IFixture fixture)
    {
        // Arrange
        var mockData = new List<(string LocalAuthorityCode, ProjectType Type)>
        {
            ("LA001", ProjectType.Transfer),
            ("LA001", ProjectType.Transfer),
            ("LA001", ProjectType.Transfer),
            ("LA002", ProjectType.Conversion),
            ("LA002", ProjectType.Conversion),
            ("LA002", ProjectType.Conversion),
            ("LA003", ProjectType.Transfer),
            ("LA003", ProjectType.Transfer),
            ("LA003", ProjectType.Transfer),
            ("LA003", ProjectType.Conversion),
        };

        var mockLocalAuthorities = new List<LocalAuthority>
        {
            fixture.Build<LocalAuthority>().With(la => la.Code, "LA001").Create(),
            fixture.Build<LocalAuthority>().With(la => la.Code, "LA002").Create(),
            fixture.Build<LocalAuthority>().With(la => la.Code, "LA003").Create()
        };


        var mockProjects = new List<Domain.Entities.Project>();
        for (int i = 0; i < 10; i++)
        {
            var proj = fixture.Create<Domain.Entities.Project>();
            proj.Type = mockData[i].Type;
            proj.LocalAuthority = mockLocalAuthorities
                .FirstOrDefault(la => la.Code == mockData[i].LocalAuthorityCode)
                ?? throw new InvalidOperationException($"Local authority with code {mockData[i].LocalAuthorityCode} not found.");
            mockProjects.Add(proj);
        }

        mockListAllProjectsWithLAsQueryService
             .ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
             .Returns(mockProjects.BuildMock());

        // Act
        var query = new ListAllProjectsByLocalAuthoritiesQuery();
        var handlerResult = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(handlerResult);
// TODO add equivalence
        Assert.Equal(3, handlerResult.ItemCount);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectLocalAuthoritiesArrangement))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        ListAllProjectByLocalAuthorities handler)
    {
        //Act
        var query = new ListAllProjectsByLocalAuthoritiesQuery { Page = 10 };

        var handlerResult = await handler.Handle(query, default);

        //Assert 

        // Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);
    }
}