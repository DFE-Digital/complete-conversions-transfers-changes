using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Contacts.Interfaces;

public interface IContactReadRepository
{
    IQueryable<Contact> Contacts { get; }
}
