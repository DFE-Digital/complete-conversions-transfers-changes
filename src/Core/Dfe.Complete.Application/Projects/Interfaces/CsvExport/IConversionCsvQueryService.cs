using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Projects.Interfaces.CsvExport
{
    public interface IConversionCsvQueryService
    {
        IQueryable<ConversionCsvModel> GetByMonth(int month, int year);
    }
}
