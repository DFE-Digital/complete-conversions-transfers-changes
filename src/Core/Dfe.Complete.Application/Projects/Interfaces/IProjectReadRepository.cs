using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface IProjectReadRepository
    {
        IQueryable<Project> Projects { get; }
        
        IQueryable<Project> ProjectsNoIncludes { get; }
        
        IQueryable<Project> ProjectsAllIncludes { get; }
    }
}
