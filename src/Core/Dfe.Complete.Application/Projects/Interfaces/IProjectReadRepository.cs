using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface IProjectReadRepository
    {
        IQueryable<Project> Projects { get; }
        
        IQueryable<Project> ProjectsNoIncludes { get; }
    }
}
