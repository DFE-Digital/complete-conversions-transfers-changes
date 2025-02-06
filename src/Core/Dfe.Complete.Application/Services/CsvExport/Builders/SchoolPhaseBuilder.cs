using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class SchoolPhaseBuilder<T>(Func<T, GiasEstablishment> selector) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            var establishment = selector(value);

            if (establishment?.PhaseName == "Not applicable")
            {
                return establishment.TypeName ?? "";
            }

            return establishment?.PhaseName ?? "";
        }
    }
}
