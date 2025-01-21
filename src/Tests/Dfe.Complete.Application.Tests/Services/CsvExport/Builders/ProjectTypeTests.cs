using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Services.CsvExport.Builders;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Tests.Common.Customizations.Behaviours;
using Dfe.Complete.Tests.Common.Customizations.Models;
using DfE.CoreLibs.Testing.AutoFixture.Attributes;
using DfE.CoreLibs.Testing.AutoFixture.Customizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Tests.Services.CsvExport.Builders
{
    public class ProjectTypeTests
    {
        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void Build_When_Conversion(Project project)
        {
            project.Type = ProjectType.Conversion;
            var builder = new ProjectTypeBuilder();

            var result = builder.Build(new ConversionCsvModel(project, null, null));

            Assert.Equal("Conversion", result);
        }

        [Theory]
        [CustomAutoData(typeof(ProjectCustomization), typeof(DateOnlyCustomization), typeof(IgnoreVirtualMembersCustomisation))]
        public void Build_When_Transfer(Project project)
        {
            project.Type = ProjectType.Transfer;
            var builder = new ProjectTypeBuilder();

            var result = builder.Build(new ConversionCsvModel(project, null, null));

            Assert.Equal("Transfer", result);
        }
    }
}
