using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Services.CsvExport
{
    public interface ICSVFileContentGenerator<TModel>
    {
        string Generate(IEnumerable<TModel> models);
    }

    public class CSVFileContentGenerator<TModel>(IHeaderGenerator<TModel> header, IRowGenerator<TModel> rowGenerator) : ICSVFileContentGenerator<TModel>
    {
        public string Generate(IEnumerable<TModel> models)
        {
            return $"{header.GenerateHeader()}\n\r{string.Join("\n\r", models.Select(rowGenerator.GenerateRow))}";
        }
    }
}
