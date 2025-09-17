using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Services.Project;

public class ProjectService : IProjectService
{
    public Dictionary<string, string>? ValidateProjectCompletion(ProjectId projectId)
    {
        if (false)
        {
            return new Dictionary<string, string>
            {
                { "ErrorKey", "ErrorMessage" }
            };
        }

        return null;
    }   
}

