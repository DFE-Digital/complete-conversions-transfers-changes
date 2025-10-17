using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Models;
using GovUK.Dfe.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Application.Tests.Services.CsvExport.Builders
{
    public class SchoolPhaseBuilderTests
    {
        [Theory]
        [CustomAutoData(typeof(GiasEstablishmentsCustomization))]
        public void WhenNotApplicableShouldReturnSchoolType(GiasEstablishment establishment)
        {
            establishment.PhaseName = "Not applicable";

            var builder = new SchoolPhaseBuilder<GiasEstablishment>(x => x);

            var result = builder.Build(establishment);

            Assert.Equal(establishment.TypeName, result);
        }

        [Theory]
        [CustomAutoData(typeof(GiasEstablishmentsCustomization))]
        public void WhenCalledShouldReturnSchoolPhase(GiasEstablishment establishment)
        {
            var builder = new SchoolPhaseBuilder<GiasEstablishment>(x => x);

            var result = builder.Build(establishment);

            Assert.Equal(establishment.PhaseName, result);
        }
    }
}
