using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class SclRepository<TAggregate>(SclContext dbContext)
        : Repository<TAggregate, SclContext>(dbContext), ISclRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
    }
}