using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto?> GetProjectById(string projectId);
    }
}
