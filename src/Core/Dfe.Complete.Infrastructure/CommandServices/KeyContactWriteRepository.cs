using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices;

internal class KeyContactWriteRepository(CompleteContext context) : IKeyContactWriteRepository
{
    private readonly CompleteContext _context = context;

    public async Task AddKeyContactAsync(KeyContact KeyContact, CancellationToken cancellationToken)
    {
        _context.KeyContacts.Add(KeyContact);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateKeyContactAsync(KeyContact KeyContact, CancellationToken cancellationToken)
    {
        _context.KeyContacts.Update(KeyContact);
        await _context.SaveChangesAsync(cancellationToken);
    }
}