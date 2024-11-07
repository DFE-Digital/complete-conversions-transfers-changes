using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.Interfaces.Repositories
{
    public interface ICompleteRepository<TAggregate> : IRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}
