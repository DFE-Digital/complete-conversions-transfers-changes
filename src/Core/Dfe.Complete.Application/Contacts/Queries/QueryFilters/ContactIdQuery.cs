using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Contacts.Queries.QueryFilters;

public class ContactIdQuery(ContactId? contactId) : IQueryObject<Contact>
{
    public IQueryable<Contact> Apply(IQueryable<Contact> query)
        => contactId == null ? query : query.Where(contact => contact.Id == contactId);
}
