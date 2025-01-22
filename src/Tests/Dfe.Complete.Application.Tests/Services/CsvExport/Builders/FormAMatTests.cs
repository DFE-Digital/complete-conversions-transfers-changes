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

    public class FormAMatTests
    {
        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization))]

        public void Build_When_Conversion(Project project)
        {
            //TODO
            //project.Type = ProjectType.Conversion;
            //var builder = new FormAMat();

            //var result = builder.Build(new ConversionCsvModel(project, null, null));

            //Assert.Equal("", result);
        }
    }
}
