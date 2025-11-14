using Dfe.Complete.Application.Users.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    public class UserReadRepository(CompleteContext ctx) : IUserReadRepository
    {
        public IQueryable<User> Users
            => ctx.Users
            .AsNoTracking();

        public async Task<User?> GetByIdAsync(UserId userId, CancellationToken cancellationToken = default)
        {
            return await ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetActiveTeamLeadersAsync(CancellationToken cancellationToken = default)
        {
            return await ctx.Users
                .AsNoTracking()
                .Where(u => u.ManageTeam == true)
                .ToListAsync(cancellationToken);
        }
    }
}
