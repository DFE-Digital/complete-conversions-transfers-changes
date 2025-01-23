using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Models;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;

namespace Dfe.Complete.Application.Tests.Services.CsvExport.Builders
{
    public class ProvisionalDateBuilderTests
    {
        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(SignificantDateHistoryCustomization))]
        public void IfHistoryDoesNotExistUseDateOnProject(Project project, SignificantDateHistory history)
        {
            var model = new ConversionCsvModel(project, null, null, null, null);

            var builder = new ProvisionalDateBuilder();

            var result = builder.Build(model);

            Assert.Equal(model.Project.SignificantDate?.ToString("dd/MM/yyyy"), result);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(SignificantDateHistoryCustomization))]

        public void IfHistoryExistsUseDateOnHistory(Project project, SignificantDateHistory history)
        {
            var model = new ConversionCsvModel(project, null, null, null, history);

            var builder = new ProvisionalDateBuilder();

            var result = builder.Build(model);

            Assert.Equal(model.SignificantDateHistory?.PreviousDate.Value.ToString("dd/MM/yyyy"), result);
        }
    }
}
