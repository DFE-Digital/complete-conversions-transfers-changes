using Dfe.Complete.Application.Services.TrustService;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using System.Reflection.Metadata.Ecma335;

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

        public RowBuilder<T> BlankIfEmpty(Func<T, object?> func)
        {
            _columnBuilders.Add(new BlankIfEmpty<T>(func));
            return this;
        }

        public RowBuilder<T> Column(IColumnBuilder<T> columnBuilder)
        {       
            _columnBuilders.Add(columnBuilder);
            return this;
        }

        public RowBuilder<T> DefaultIf(Func<T, bool> condition, Func<T, object?> valueFunc, string defaultValue)
        {
            _columnBuilders.Add(new DefaultIf<T>(condition, valueFunc, defaultValue));
            return this;
        }

        public RowBuilder<T> TrustData(Func<T, Ukprn> ukprn, Func<TrustDetailsDto, string> value)
        {
            _columnBuilders.Add(new TrustDataBuilder<T>(trustCache, ukprn, value));
            return this;
        }
            
        public string Build(T model)
        {
            return _columnBuilders.Select(x => x.Build(model)).Aggregate((acc, x) => acc + "," + x);
        }

    }

    
}
