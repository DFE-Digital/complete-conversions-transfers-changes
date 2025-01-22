using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class FormAMat() : IColumnBuilder<ConversionCsvModel>
    {
        public string Build(ConversionCsvModel input)
        {
            var projectType = input.Project.Type;

            if (projectType == Domain.Enums.ProjectType.Conversion)
            {
                return "join a MAT";
            }
            else
            {
                return "form a MAT";
            }
        }
    }
}
