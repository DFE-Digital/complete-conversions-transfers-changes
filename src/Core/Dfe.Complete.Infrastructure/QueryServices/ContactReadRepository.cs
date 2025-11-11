using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ContactReadRepository(CompleteContext context) : IContactReadRepository
{
    public IQueryable<Contact> Contacts => context.Contacts.AsNoTracking();
}
