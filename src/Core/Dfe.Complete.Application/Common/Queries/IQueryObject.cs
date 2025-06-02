namespace Dfe.Complete.Application.Common.Queries
{
    public interface IQueryObject<T>
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
