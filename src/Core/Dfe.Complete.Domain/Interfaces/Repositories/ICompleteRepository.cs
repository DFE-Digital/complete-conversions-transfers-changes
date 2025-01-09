using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Domain.Interfaces.Repositories;

public interface ICompleteRepository<TAggregate> : IRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
{
    Task<ProjectGroupId?> GetProjectGroupIdByIdentifierAsync(string groupIdentifier,
        CancellationToken cancellationToken);

    Task<User?> GetUserByAdId(string? userAdId, CancellationToken cancellationToken);
}