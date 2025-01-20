using System.Diagnostics.CodeAnalysis;
using Dfe.Complete.Domain.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.Repositories
{
    // same as the interface, this should not contain any methods, please move the methods into their own repository

    // Aggregate root (Project) should have its own repository e.g. ProjectRepository.cs  please refer to
    // https://github.com/DFE-Digital/rsd-ddd-clean-architecture/blob/main/src/DfE.DomainDrivenDesignTemplate.Infrastructure/Repositories/SchoolRepository.cs
    // to see how it is implemented

    [ExcludeFromCodeCoverage]
    public class CompleteRepository<TAggregate>(CompleteContext dbContext) : Repository<TAggregate, CompleteContext>(dbContext), ICompleteRepository<TAggregate>
        where TAggregate : class, IAggregateRoot
    {
        private readonly CompleteContext _dbContext = dbContext;

        // if the repository method or the query is not complex, please use the generic repository, this reduces code duplication and less coupling with other classes,
        // As mentioned in the ProjectGroup aggregate class this need to be an aggregate root, either to have its own repository or use the generic repo 

        public async Task<ProjectGroupId?> GetProjectGroupIdByIdentifierAsync(string groupIdentifier,
            CancellationToken cancellationToken)
        {
            var projectGroup = await _dbContext.ProjectGroups
                .AsNoTracking()
                .Where(g => g.GroupIdentifier == groupIdentifier)
                .FirstOrDefaultAsync(cancellationToken);

            return projectGroup?.Id; 
        }
        
        // what do we need from this table, can this be taken from the AD groups/claims?

        public async Task<User?> GetUserByAdId(string? userAdId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.ActiveDirectoryUserId == userAdId)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}