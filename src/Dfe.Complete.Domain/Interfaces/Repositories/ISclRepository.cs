using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.Interfaces.Repositories
{
    public interface ISclRepository<TAggregate> : IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}
