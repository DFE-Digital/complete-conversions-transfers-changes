using Dfe.Complete.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class DfeNumberLAESTABBuilder: IColumnBuilder<ConversionCsvModel>
    {
        public string Build(ConversionCsvModel input)
        {
            if(input.Project.AcademyUrn == null)
            {
                return string.Empty;
            }

            if (input.Academy == null)
            {
                return string.Empty;
            }

            return input.Academy.LocalAuthorityCode + "/" + input.Academy.EstablishmentNumber;
        }
    }
}
