using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface IKeyContactReadRepository
    {
        IQueryable<KeyContact> KeyContacts { get; }
    }
}
