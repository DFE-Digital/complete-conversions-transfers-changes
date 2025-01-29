namespace Dfe.Complete.Application.Services.CsvExport
{
    public interface IRowGenerator<TModel>
    {
        public string GenerateRow(TModel model);
    }
}
