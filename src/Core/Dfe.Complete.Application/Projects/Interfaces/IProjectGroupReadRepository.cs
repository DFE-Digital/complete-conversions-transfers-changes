using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface IProjectGroupReadRepository
    {
        IQueryable<ProjectGroup> ProjectGroups { get; }
    }
}
