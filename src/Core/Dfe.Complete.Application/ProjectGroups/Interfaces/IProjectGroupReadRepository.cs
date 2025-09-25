using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.ProjectGroups.Interfaces
{
    public interface IProjectGroupReadRepository
    {
        IQueryable<ProjectGroup> ProjectGroups { get; }
    }
}
