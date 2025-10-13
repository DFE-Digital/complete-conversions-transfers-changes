using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.KeyContacts.Interfaces;

public interface IKeyContactWriteRepository
{   
    Task UpdateKeyContactAsync(KeyContact KeyContact, CancellationToken cancellationToken);
}