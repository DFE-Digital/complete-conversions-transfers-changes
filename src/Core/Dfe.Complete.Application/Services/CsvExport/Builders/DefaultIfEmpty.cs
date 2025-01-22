namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class DefaultIf<T>(Func<T, bool> condition, Func<T, object?> valueFunc, string defaultValue) : IColumnBuilder<T>
    {
        public string Build(T input)
        {
            if (condition(input))
            {
                return defaultValue;
            }
            else
            {
                object? value = valueFunc(input);
                return value.ToString();
            }
        }
    }
}
