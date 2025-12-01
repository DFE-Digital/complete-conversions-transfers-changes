using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateAssignedUserCommand(
    ProjectId ProjectId,
    UserId AssignedUser
) : IRequest;

public class UpdateAssignedUser(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository)
    : IRequestHandler<UpdateAssignedUserCommand>
{
    public async Task Handle(UpdateAssignedUserCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken);
        var user = await userRepository.GetAsync(request.AssignedUser, cancellationToken);
        if (user is null || !user.IsAssignableToProject)
        {
            throw new NotFoundException("Email is not assignable", "email");
        }

        // Only raise event if assignment is changing
        if (project.AssignedToId != request.AssignedUser)
        {
            var schoolName = project.GiasEstablishment?.Name ?? $"School URN {project.Urn.Value}";
            project.RaiseProjectAssignedToUserEvent(
                project.Id.Value.ToString(),
                request.AssignedUser,
                user.Email ?? string.Empty,
                user.FirstName ?? string.Empty,
                schoolName);
        }

        project.AssignedAt = DateTime.UtcNow;
        project.AssignedToId = request.AssignedUser;
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}