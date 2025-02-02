namespace Dfe.Complete.Application.Services.CsvExport
{
    public interface IHeaderGenerator<TModel>
    {
        string GenerateHeader();
    }
}
