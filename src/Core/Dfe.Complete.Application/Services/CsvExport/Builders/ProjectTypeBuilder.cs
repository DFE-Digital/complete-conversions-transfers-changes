using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class ProjectTypeBuilder: IColumnBuilder<ConversionCsvModel>
    {
        public string Build(ConversionCsvModel input)
        {
            var projectType = input.Project.Type;

            if (projectType == Domain.Enums.ProjectType.Conversion)
            {
                return "Conversion";
            }
            else
            {
                return "Transfer";
            }
        }
    }
}
