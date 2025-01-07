using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CompleteRepository<TAggregate>(CompleteContext dbContext) : Repository<TAggregate, CompleteContext>(dbContext), ICompleteRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        private readonly CompleteContext _dbContext = dbContext;

        public async Task<ProjectGroupId?> GetProjectGroupIdByIdentifierAsync(string groupIdentifier,
            CancellationToken cancellationToken)
        {
            var projectGroup = await _dbContext.ProjectGroups
                .AsNoTracking()
                .Where(g => g.GroupIdentifier == groupIdentifier)
                .FirstOrDefaultAsync(cancellationToken);

            return projectGroup?.Id; 
        }

        public async Task<User?> GetUserByEmail(string? email, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}