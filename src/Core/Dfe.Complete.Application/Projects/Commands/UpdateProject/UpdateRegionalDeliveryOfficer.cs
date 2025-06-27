using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateRegionalDeliveryOfficerCommand(
    ProjectId ProjectId,
    UserId RegionalDeliveryOfficer
    ) : IRequest<Result<bool>>;

public class UpdateRegionalDeliveryOfficer(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository)
    : IRequestHandler<UpdateRegionalDeliveryOfficerCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateRegionalDeliveryOfficerCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken);
        var user = await userRepository.GetAsync(request.RegionalDeliveryOfficer, cancellationToken);
        if (user is null || user.AssignToProject is false)
        {
            throw new NotFoundException("Email is not assignable", "email");
        }
        project.RegionalDeliveryOfficerId = request.RegionalDeliveryOfficer;
        project.RegionalDeliveryOfficer = user;
        var success = await projectRepository.UpdateAsync(project, cancellationToken);
        return success.RegionalDeliveryOfficerId != request.RegionalDeliveryOfficer ? Result<bool>.Failure("User has not been updated") : Result<bool>.Success(true);
    }
}