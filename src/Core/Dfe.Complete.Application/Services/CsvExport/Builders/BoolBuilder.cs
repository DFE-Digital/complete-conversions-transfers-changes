namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public class BoolBuilder<T>(Func<T, bool?> func, string trueValue, string falseValue) : IColumnBuilder<T>
    {
        public string Build(T value)
        {
            if(func(value) == true)
                return trueValue;

            return falseValue;
        }
    }
}
