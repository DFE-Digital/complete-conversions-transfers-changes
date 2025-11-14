using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Users.Interfaces
{
    public interface IUserReadRepository
    {
        IQueryable<User> Users { get; }
        
        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets all active team leaders (users with manage_team permission who are active).
        /// </summary>
        Task<IEnumerable<User>> GetActiveTeamLeadersAsync(CancellationToken cancellationToken = default);
    }
}
