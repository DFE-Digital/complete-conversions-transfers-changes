using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;
using Dfe.Complete.Application.Projects.Queries.ProjectsByRegion;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using MockQueryable;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectLocalAuthoritiesQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(ListAllProjectsQueryModelCustomization),
        typeof(DateOnlyCustomization))]
    public async Task Handle_ShouldReturnListProjectsLocalAuthorities(
        [Frozen] IListAllProjectsQueryService mockListAllProjectsQueryService,
        [Frozen] ICompleteRepository<LocalAuthority> localAuthoritiesRepo,
        ListAllProjectLocalAuthorities handler,
        IFixture fixture)
    {
        var localAuthorities = fixture.CreateMany<LocalAuthority>(20);
        var localAuthorityCode = localAuthorities.FirstOrDefault().Code;

        var expectedLocalAuthorityCodes = localAuthorities.SelectMany(la => la.Code).Take(10);

        localAuthoritiesRepo.FetchAsync(Arg.Any<Expression<Func<LocalAuthority, bool>>>(), default)
            .Returns(localAuthorities.ToList());

        var listAllProjectsQueryModels = fixture.CreateMany<ListAllProjectsQueryModel>(50).ToList();
        listAllProjectsQueryModels.Take(30).ToList()
            .ForEach(p => p.Establishment.LocalAuthorityCode = localAuthorityCode);

        var mockListAllProjects = listAllProjectsQueryModels.BuildMock();

        mockListAllProjectsQueryService.ListAllProjects(Arg.Any<ProjectState?>(), Arg.Any<ProjectType?>())
            .Returns(mockListAllProjects);

        var expectedProjects =
            listAllProjectsQueryModels.Where(p => p.Establishment.LocalAuthorityCode == localAuthorityCode).ToList();

        var expected = new List<ListAllProjectLocalAuthoritiesResultModel>();
        expected.AddRange(localAuthorities
            .Where(la => la.Code == localAuthorityCode)
            .Select(la => new ListAllProjectLocalAuthoritiesResultModel(
                la,
                la.Code,
                expectedProjects.Count(p => p.Project.Type == ProjectType.Conversion),
                expectedProjects.Count(p => p.Project.Type == ProjectType.Transfer))));

        var query = new ListAllProjectLocalAuthoritiesQuery();

        var handlerResult = await handler.Handle(query, default);
    }
}