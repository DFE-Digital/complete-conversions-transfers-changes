using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.Interfaces.Repositories;

// This is interface to the generic repository, it should not contain any methods!
// Please remove the methods and either use the generic version of them or create them in their own repo

public interface ICompleteRepository<TAggregate> : IRepository<TAggregate> where TAggregate : class, IAggregateRoot
{
}