using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Contacts.Interfaces;

public interface IContactWriteRepository
{
    Task CreateContactAsync(Contact Contact, CancellationToken cancellationToken);
    Task RemoveContactAsync(Contact Contact, CancellationToken cancellationToken);
    Task UpdateContactAsync(Contact Contact, CancellationToken cancellationToken);
}