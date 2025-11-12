using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateAssignedUserCommand(
    ProjectId ProjectId,
    UserId AssignedUser
) : IRequest<Result<bool>>;

public class UpdateAssignedUser(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository)
    : IRequestHandler<UpdateAssignedUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateAssignedUserCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken);
        var user = await userRepository.GetAsync(request.AssignedUser, cancellationToken);
        if (user is null || user.AssignToProject is false)
        {
            return Result<bool>.Failure("Email is not assignable", ErrorType.NotFound); 
        }

        project.AssignedAt = DateTime.UtcNow;
        project.AssignedToId = request.AssignedUser;
        await projectRepository.UpdateAsync(project, cancellationToken);
        
        return Result<bool>.Success(true);
    }
}