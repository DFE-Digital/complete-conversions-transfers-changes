using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Constants;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using AllProjectsInTrustViewModel = Dfe.Complete.Pages.Projects.List.AllProjectsInTrust.AllProjectsInTrustViewModel;

namespace Dfe.Complete.Tests.Pages.Projects.List;

public class AllProjectsInTrustViewModelTests
{
    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetTrustProjectsUrl_ShouldReturnCorrectMatUrl_When_IdentifierIsTrustReference(IFixture fixture)
    {
        var trust = fixture.Build<ListTrustsWithProjectsResultModel>()
            .With(x => x.Identifier, "TR00001")
            .Create();

        string expectedUrl = "/projects/all/trusts/reference/TR00001";

        // Act
        var result = AllProjectsInTrustViewModel.GetTrustProjectsUrl(trust);

        // Assert
        Assert.Equal(expectedUrl, result);
    }

    [Theory]
    [CustomAutoData(typeof(ListAllProjectResultModelCustomization))]
    public void GetTrustProjectsUrl_ShouldReturnCorrectUrl_When_IdentifierIsNotTrustReference(IFixture fixture)
    {
        var trust = fixture.Build<ListTrustsWithProjectsResultModel>()
            .With(x => x.Identifier, "10035415")
            .Create();

        string expectedUrl = "/projects/all/trusts/ukprn/10035415";

        // Act
        var result = AllProjectsInTrustViewModel.GetTrustProjectsUrl(trust);

        // Assert
        Assert.Equal(expectedUrl, result);
    }
}