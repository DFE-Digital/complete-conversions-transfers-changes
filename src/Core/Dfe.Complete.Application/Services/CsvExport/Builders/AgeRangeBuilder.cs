using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class AgeRangeBuilder<T>(Func<T, GiasEstablishment> selector): IColumnBuilder<T>
    {
        public string Build(T value)
        {
            var establishment = selector(value);

            if (establishment.AgeRangeLower == null || establishment.AgeRangeUpper == null)
            {
                return string.Empty;
            }

            return $"{establishment.AgeRangeLower}-{establishment.AgeRangeUpper}";
        }
    }
}
