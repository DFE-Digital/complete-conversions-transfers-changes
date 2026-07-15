namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class DefaultIfEmpty<T>(Func<T, object?> func, string defaultValue) : IColumnBuilder<T>
    {
        public string Build(T input)
        {
            object? value = func(input);

            if (value == null || value.ToString() == string.Empty)
            {
                return defaultValue;
            }

            return value.ToString();
        }
    }
}
