namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public interface IColumnBuilder<in T>
    {
        string Build(T input);
    }
}
