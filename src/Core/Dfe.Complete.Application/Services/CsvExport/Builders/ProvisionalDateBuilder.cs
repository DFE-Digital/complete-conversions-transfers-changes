using Dfe.Complete.Application.Common.Models;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class ProvisionalDateBuilder: IColumnBuilder<ConversionCsvModel>
    {
        public string Build(ConversionCsvModel model)
        {
            if(model.SignificantDateHistory == null)
                return model.Project.SignificantDate?.ToString("yyyy-MM-dd") ?? string.Empty;

            return model.SignificantDateHistory.PreviousDate.Value.ToString("yyyy-MM-dd") ?? string.Empty;
        }
    }
}
