using Dfe.Complete.Application.ProjectGroups.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices;

internal class ProjectGroupWriteRepository(CompleteContext context) : IProjectGroupWriteRepository
{
    private readonly CompleteContext _context = context;

    public async Task<ProjectGroup> CreateProjectGroupAsync(ProjectGroup projectGroup, CancellationToken cancellationToken)
    {
        await _context.ProjectGroups.AddAsync(projectGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return projectGroup; // TODO remove
    }
}