using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
