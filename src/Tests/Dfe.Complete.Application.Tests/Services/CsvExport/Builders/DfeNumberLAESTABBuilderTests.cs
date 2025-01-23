using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Application.Tests.Services.CsvExport.Builders
{

    public class DfeNumberLAESTABBuilderTests
    {
        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(EstablishmentsCustomization))]

        public void BuildsCorrectDfeNumber(Project project, GiasEstablishment academy)
        {
            var builder = new DfeNumberLAESTABBuilder();

            var result = builder.Build(new ConversionCsvModel(project, null, academy, null, null));

            Assert.Equal(academy.LocalAuthorityCode + "/" + academy.EstablishmentNumber, result);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(EstablishmentsCustomization))]
        public void BuildBlankIfEmpty(Project project, GiasEstablishment academy)
        {
            project.AcademyUrn = null;
            var builder = new DfeNumberLAESTABBuilder();

            var result = builder.Build(new ConversionCsvModel(project, null, academy, null, null));

            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(EstablishmentsCustomization))]
        public void BuildBlankIfAcademyNotFound(Project project)
        {
            project.AcademyUrn = null;
            var builder = new DfeNumberLAESTABBuilder();

            var result = builder.Build(new ConversionCsvModel(project, null, null, null, null));

            Assert.Equal(string.Empty, result);
        }
    }
}
