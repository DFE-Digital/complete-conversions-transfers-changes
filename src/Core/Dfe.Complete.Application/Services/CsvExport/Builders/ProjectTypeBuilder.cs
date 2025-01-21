using Dfe.Complete.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
