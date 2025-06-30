using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateAssignedUserCommand(
    Urn ProjectUrn,
    UserId AssignedUser
    ) : IRequest;

public class UpdateAssignedUser(
    ICompleteRepository<Project> projectRepository, 
    ICompleteRepository<User> userRepository)
    : IRequestHandler<UpdateAssignedUserCommand>
{
    public async Task Handle(UpdateAssignedUserCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Urn == request.ProjectUrn, cancellationToken);
        var user = await userRepository.GetAsync(request.AssignedUser, cancellationToken);
        if (user is null || user.AssignToProject is false)
        {
            throw new NotFoundException("Email is not assignable", "email");
        }
        project.AssignedAt = DateTime.UtcNow;
        project.AssignedToId = request.AssignedUser;
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}