using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using NSubstitute;

namespace Dfe.Complete.Application.Tests.Services.CsvExport.Builders
{

    public class TrustDataBuilderTests
    {

        [Theory]
        [CustomAutoData(typeof(TrustDetailsDtoCustomization))]
        public void ShouldBeAbleToGetFromTrust(TrustDetailsDto trust)
        {
            var TrustCache = Substitute.For<ITrustCache>();
            TrustCache.GetTrustAsync(trust.Ukprn).Returns(trust);
            var TrustData = new TrustDataBuilder<TrustTestModel>(TrustCache, u => u.id, t => t.CompaniesHouseNumber);

            var result = TrustData.Build(new TrustTestModel(trust.Ukprn));

            Assert.Equal(trust.CompaniesHouseNumber, result);
        }

        [Theory]
        [CustomAutoData(typeof(TrustDetailsDtoCustomization))]
        public void ShouldBeBlankIfTrustNotFound(TrustDetailsDto trust)
        {
            var TrustCache = Substitute.For<ITrustCache>();

            var TrustData = new TrustDataBuilder<TrustTestModel>(TrustCache, u => u.id, t => t.CompaniesHouseNumber);

            var result = TrustData.Build(new TrustTestModel(trust.Ukprn));

            Assert.Equal(string.Empty, result);
        }
    }
}
