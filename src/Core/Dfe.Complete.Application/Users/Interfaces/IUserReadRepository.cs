using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Users.Interfaces
{
    public interface IUserReadRepository
    {
        IQueryable<User> Users { get; }
    }
}
