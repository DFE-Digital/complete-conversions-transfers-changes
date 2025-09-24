using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices
{
    internal class ProjectWriteRepository(CompleteContext context) : IProjectWriteRepository
    {
        public async Task UpdateProjectAsync(Project project, CancellationToken cancellationToken)
        {
            context.Projects.Update(project);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
