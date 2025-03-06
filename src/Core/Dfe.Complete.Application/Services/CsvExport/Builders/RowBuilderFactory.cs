using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Services.TrustCache;
using Dfe.Complete.Domain.Entities;
using System.Text;

namespace Dfe.Complete.Application.Services.CsvExport.Builders
{
    public interface IRowBuilderFactory<T>
    {
        RowBuilder<T> DefineRow();
    }

    public class RowBuilderFactory<T>(ITrustCache trustCache) : IRowBuilderFactory<T>
    {
        public RowBuilder<T> DefineRow() => new RowBuilder<T>(trustCache);
    }

    public class RowBuilder<T>(ITrustCache trustCache)
    {
        private List<IColumnBuilder<T>> _columnBuilders = new();
        private StringBuilder _headers = new();
        private bool _firstHeader = true;

        public RowBuilder<T> Column(string name)
        {
            if(!_firstHeader)
            {
                _headers.Append(",");
            }
            else
            {
                _firstHeader = false;
            }

            _headers.Append(name);
            return this;
        }
       
        public RowBuilder<T> BlankIfEmpty(Func<T, object?> func)
        {
            _columnBuilders.Add(new BlankIfEmpty<T>(func));
            return this;
        }

        public RowBuilder<T> Builder(IColumnBuilder<T> columnBuilder)
        {       
            _columnBuilders.Add(columnBuilder);
            return this;
        }

        public RowBuilder<T> DefaultIf(Func<T, bool> condition, Func<T, string?> valueFunc, string defaultValue)
        {
            _columnBuilders.Add(new DefaultIf<T>(condition, valueFunc, defaultValue));
            return this;
        }

        public RowBuilder<T> DefaultIfEmpty(Func<T, object?> func, string defaultValue)
        {
            _columnBuilders.Add(new DefaultIfEmpty<T>(func, defaultValue));
            return this;
        }

        public RowBuilder<T> Bool(Func<T, bool?> condition, string trueValue, string falseValue)
        {
            _columnBuilders.Add(new BoolBuilder<T>(condition, trueValue, falseValue));
            return this;
        }

        public RowBuilder<T> IncomingTrustData(Func<T, Project> project, Func<TrustDto, string> value)
        {
            _columnBuilders.Add(new IncomingTrustDataBuilder<T>(trustCache, project, value));
            return this;
        }
            
        public string Build(T model)
        {
            return _columnBuilders.Select(x => x.Build(model)).Aggregate((acc, x) => acc + "," + x);
        }

        public string BuildHeaders()
        {
            return _headers.ToString();
        }

    }

    
}
