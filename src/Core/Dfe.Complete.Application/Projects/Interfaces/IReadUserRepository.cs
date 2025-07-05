using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface IReadUserRepository
    {
        IQueryable<User> Users { get; }
    }
}
