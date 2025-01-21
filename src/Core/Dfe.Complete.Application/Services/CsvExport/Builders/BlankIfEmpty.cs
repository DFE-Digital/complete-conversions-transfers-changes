namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class BlankIfEmpty<T>(Func<T, object?> func) : IColumnBuilder<T>
    {
        public string Build(T input)
        {
            object? value = func(input);

            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
