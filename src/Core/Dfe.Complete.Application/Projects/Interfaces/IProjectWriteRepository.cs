using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface IProjectWriteRepository
    {
        Task UpdateProjectAsync(Project project, CancellationToken cancellationToken);
    }
}
