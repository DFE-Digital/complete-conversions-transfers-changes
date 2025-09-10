using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ListAllProjects;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectByLocalAuthoritiesQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
    typeof(OmitCircularReferenceCustomization),
    typeof(DateOnlyCustomization),
    typeof(ProjectsQueryBuilderCustomization),
    typeof(ProjectCustomization))]
    public async Task Handle_ShouldReturnListProjectsLocalAuthorities(
    [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
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
            fixture.Build<LocalAuthority>()
               .With(la => la.Code, "LA001")
               .With(la => la.Name, "LA001")
               .Create(),
            fixture.Build<LocalAuthority>()
               .With(la => la.Code, "LA002")
               .With(la => la.Name, "LA002")
               .Create(),
            fixture.Build<LocalAuthority>()
               .With(la => la.Code, "LA003")
               .With(la => la.Name, "LA003")
               .Create()
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

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .GetProjects()
            .Returns(mockProjects.BuildMock());

        // Act
        var query = new ListAllProjectsByLocalAuthoritiesQuery();
        var handlerResult = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(handlerResult);
        Assert.Equal(3, handlerResult.ItemCount);

        Assert.Equal("LA001", handlerResult.Value?[0].LocalAuthorityCode);
        Assert.Equal(3, handlerResult.Value?[0].Transfers);
        Assert.Equal(0, handlerResult.Value?[0].Conversions);

        Assert.Equal("LA002", handlerResult.Value?[1].LocalAuthorityCode);
        Assert.Equal(0, handlerResult.Value?[1].Transfers);
        Assert.Equal(3, handlerResult.Value?[1].Conversions);

        Assert.Equal("LA003", handlerResult.Value?[2].LocalAuthorityCode);
        Assert.Equal(3, handlerResult.Value?[2].Transfers);
        Assert.Equal(1, handlerResult.Value?[2].Conversions);
    }

    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ProjectsQueryBuilderCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenAllPagesAreSkipped(
        [Frozen] IProjectsQueryBuilder mockProjectsQueryBuilder,
        ListAllProjectByLocalAuthorities handler,
        IFixture fixture)

    {
        // Arrange
        var mockProjects = new List<Domain.Entities.Project>();
        var mockLocalAuthoritY = fixture.Build<LocalAuthority>().Create();

        for (int i = 0; i < 10; i++)
        {
            var proj = fixture.Create<Domain.Entities.Project>();
            proj.LocalAuthority = mockLocalAuthoritY;
            mockProjects.Add(proj);
        }

        mockProjectsQueryBuilder
            .ApplyProjectFilters(Arg.Any<ProjectFilters>())
            .GetProjects()
            .Returns(mockProjects.BuildMock());

        //Act
        var query = new ListAllProjectsByLocalAuthoritiesQuery { Page = 10 };

        var handlerResult = await handler.Handle(query, default);

        // Assert
        Assert.NotNull(handlerResult);
        Assert.True(handlerResult.IsSuccess);
        Assert.Equal(0, handlerResult.Value?.Count);
    }
}