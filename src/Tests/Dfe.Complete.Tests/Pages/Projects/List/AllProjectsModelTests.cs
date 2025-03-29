using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.List;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Tests.Pages.Projects.List;

public class AllProjectsModelTests
{
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetTrustProjectsUrl_ShouldReturnCorrectMatUrl_When_IdentifierIsTrustReference(IFixture fixture)
    {
        var trust = fixture.Build<ListTrustsWithProjectsResultModel>()
            .With(x => x.identifier, "TR00001")
            .Create();

        string expectedUrl = string.Format(RouteConstants.TrustMATProjects, trust.identifier);

        // Act
        var result = AllProjectsModel.GetTrustProjectsUrl(trust);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
    
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetTrustProjectsUrl_ShouldReturnCorrectUrl_When_IdentifierIsNotTrustReference(IFixture fixture)
    {
        var trust = fixture.Build<ListTrustsWithProjectsResultModel>()
            .With(x => x.identifier, "10035415")
            .Create();

        string expectedUrl = string.Format(RouteConstants.TrustProjects, trust.identifier);

        // Act
        var result = AllProjectsModel.GetTrustProjectsUrl(trust);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
}