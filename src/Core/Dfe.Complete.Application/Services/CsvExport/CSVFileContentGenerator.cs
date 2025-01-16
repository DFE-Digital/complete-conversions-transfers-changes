using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Services.CsvExport
{
    public interface HeaderGenerator<TModel>
    {
        string GenerateHeader();
    }

    public interface RowGenerator<TModel>
    {
        string GenerateRow(TModel model);
    }


    public class CSVFileContentGenerator<TModel>(HeaderGenerator<TModel> header, RowGenerator<TModel> rowGenerator)
    {
        public string Generate(IEnumerable<TModel> models)
        {
            return $"{header.GenerateHeader()}\n\r{string.Join("\n\r", models.Select(rowGenerator.GenerateRow))}";
        }
    }
}
