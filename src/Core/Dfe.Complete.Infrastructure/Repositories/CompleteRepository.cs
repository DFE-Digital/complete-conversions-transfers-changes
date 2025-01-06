using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CompleteRepository<TAggregate>(CompleteContext dbContext)
        : Repository<TAggregate, CompleteContext>(dbContext), ICompleteRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        private readonly CompleteContext _dbContext = dbContext;

        public async Task<ProjectGroupId?> GetProjectGroupIdByIdentifierAsync(string groupIdentifier,
            CancellationToken cancellationToken)
        {
            var result = await _dbContext.ProjectGroups
                .AsNoTracking()
                .Where(g => g.GroupIdentifier == groupIdentifier)
                .FirstOrDefaultAsync(cancellationToken);

            return result?.Id; 
        }
    }
}