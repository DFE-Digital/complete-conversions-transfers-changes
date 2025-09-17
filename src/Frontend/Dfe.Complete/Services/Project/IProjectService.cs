using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Services.Project
{
    public interface IProjectService
    {
        Dictionary<string, string>? ValidateProjectCompletion(ProjectId projectId);
    }
}
