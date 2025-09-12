using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices;

internal class ContactWriteRepository(CompleteContext context) : IContactWriteRepository
{
    private readonly CompleteContext _context = context;

    public async Task CreateContactAsync(Contact Contact, CancellationToken cancellationToken)
    {
        await _context.Contacts.AddAsync(Contact, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveContactAsync(Contact Contact, CancellationToken cancellationToken)
    {
        _context.Contacts.Remove(Contact);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateContactAsync(Contact Contact, CancellationToken cancellationToken)
    {
        _context.Contacts.Update(Contact);
        await _context.SaveChangesAsync(cancellationToken);
    }
}