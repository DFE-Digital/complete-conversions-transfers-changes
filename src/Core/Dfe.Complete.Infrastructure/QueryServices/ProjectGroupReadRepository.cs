using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices
{
    internal class ProjectGroupReadRepository(CompleteContext ctx) : IProjectGroupReadRepository
    {
        public IQueryable<ProjectGroup> ProjectGroups =>
            ctx.ProjectGroups
                .AsNoTracking();
    }
}
