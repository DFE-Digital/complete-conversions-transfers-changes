using AutoFixture;
using AutoFixture.Xunit2;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Queries.ListAllProjectsForLocalAuthority;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.QueryHandlers.Project;

public class ListAllProjectsForLAQueryHandlerTests
{
    [Theory]
    [CustomAutoData(
        typeof(OmitCircularReferenceCustomization),
        typeof(DateOnlyCustomization),
        typeof(ListAllProjectLocalAuthoritiesCustomization))]
    public async Task Handle_ShouldReturnCorrectList_WhenPaginationIsCorrect(
        ListAllProjectsForLocalAuthority handler,
        IFixture fixture,
        List<LocalAuthority> localAuthorities)
    {
        //Arrange 
        var specificLa = localAuthorities.First();

        //Act
        var handlerResult = await handler.Handle(new ListAllProjectsForLocalAuthorityQuery(specificLa.Code), default);
    }
}