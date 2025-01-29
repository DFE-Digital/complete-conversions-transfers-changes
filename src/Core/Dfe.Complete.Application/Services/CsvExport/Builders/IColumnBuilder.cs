namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public interface IColumnBuilder<T>
    {
        string Build(T value);
    }
}
