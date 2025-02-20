namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class BlankIfEmpty<T>(Func<T, object?> func) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            object? valueObj = func(value);

            if (valueObj == null)
            {
                return string.Empty;
            }
            else
            {
                return valueObj.ToString();
            }
        }
    }
}
