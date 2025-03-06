using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class RPAOptionBuilder<T>(Func<T, RiskProtectionArrangementOption?> selector) : IColumnBuilder<T> where T : class
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
