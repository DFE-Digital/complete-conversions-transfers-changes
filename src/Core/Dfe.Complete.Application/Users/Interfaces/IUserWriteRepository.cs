using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Users.Interfaces;

public interface IUserWriteRepository
{
    Task CreateUserAsync(User user, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
}