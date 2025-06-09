using Dfe.Complete.Application.Projects.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateAssignedUserCommand(
    Urn ProjectUrn,
    UserId AssignedUser
    ) : IRequest;

public class UpdateAssignedUser(
    ICompleteRepository<Project> projectRepository,
    ICreateProjectCommon createProjectCommon)
    : IRequestHandler<UpdateAssignedUserCommand>
{
    public async Task Handle(UpdateAssignedUserCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Urn == request.ProjectUrn, cancellationToken);
        project.AssignedAt = DateTime.UtcNow;
        project.AssignedToId = request.AssignedUser;
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}