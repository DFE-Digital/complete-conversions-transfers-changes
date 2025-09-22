using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.ProjectGroups.Interfaces;

public interface IProjectGroupWriteRepository
{
    Task CreateProjectGroupAsync(ProjectGroup projectGroup, CancellationToken cancellationToken);
}
