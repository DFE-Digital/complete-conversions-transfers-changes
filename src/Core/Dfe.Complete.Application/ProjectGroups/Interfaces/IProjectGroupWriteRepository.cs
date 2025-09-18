using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.ProjectGroups.Interfaces;

public interface IProjectGroupWriteRepository
{
    Task<ProjectGroup> CreateProjectGroupAsync(ProjectGroup projectGroup, CancellationToken cancellationToken);
}
