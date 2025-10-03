using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Interfaces
{
    public interface IUserReadRepository
    {
        IQueryable<User> Users { get; }
    }
}
