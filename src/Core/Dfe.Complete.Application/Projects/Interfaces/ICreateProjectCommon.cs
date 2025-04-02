using Dfe.Complete.Application.Projects.Commands.CreateProject;

namespace Dfe.Complete.Application.Projects.Common;

public interface ICreateProjectCommon
{
    Task<CreateProjectCommonResult>  CreateCommonProject(CreateProjectCommonCommand request,
        CancellationToken cancellationToken);
}