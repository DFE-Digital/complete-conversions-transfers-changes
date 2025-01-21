using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;

namespace Dfe.Complete.Application.Tests.Services.CsvExport.Builders
{
    
    public class DfeNumberLAESTABBuilderTests
    {
        [Theory]
        [CustomAutoData(typeof(EstablishmentsCustomization))]

        public void BuildsCorrectDfeNumber(GiasEstablishment academy)
        {
            var builder = new DfeNumberLAESTABBuilder();

            var result = builder.Build(new ConversionCsvModel(null, null, academy));

            Assert.Equal(academy.LocalAuthorityCode + "/" + academy.EstablishmentNumber, result);
        }
    }
}
