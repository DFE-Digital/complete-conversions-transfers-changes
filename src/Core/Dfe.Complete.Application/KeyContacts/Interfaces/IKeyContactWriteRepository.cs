using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.KeyContacts.Interfaces;

public interface IKeyContactWriteRepository
{
    Task AddKeyContactAsync(KeyContact KeyContact, CancellationToken cancellationToken);
    Task UpdateKeyContactAsync(KeyContact KeyContact, CancellationToken cancellationToken);
}