using Dfe.Complete.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class RPAOptionBuilder<T>(Func<T, RiskProtectionArrangementOption?> selector) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            var option = value != null ? selector(value) : RiskProtectionArrangementOption.Standard;

            return option switch
            {
                RiskProtectionArrangementOption.Standard => "standard",
                RiskProtectionArrangementOption.ChurchOrTrust => "church or trust",
                RiskProtectionArrangementOption.Commercial => "commercial",
                _ => "standard",
            };
        }
    }
}
