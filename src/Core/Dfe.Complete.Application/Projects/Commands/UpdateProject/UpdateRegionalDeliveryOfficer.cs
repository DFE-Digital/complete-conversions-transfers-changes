using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateRegionalDeliveryOfficerCommand(
    Urn ProjectUrn,
    UserId RegionalDeliveryOfficer
    ) : IRequest;

public class UpdateRegionalDeliveryOfficer(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository)
    : IRequestHandler<UpdateRegionalDeliveryOfficerCommand>
{
    public async Task Handle(UpdateRegionalDeliveryOfficerCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Urn == request.ProjectUrn, cancellationToken);
        var user = await userRepository.GetAsync(request.RegionalDeliveryOfficer, cancellationToken);
        if (user is null || user.AssignToProject is false)
        {
            throw new NotFoundException("Email is not assignable", "email");
        }
        project.RegionalDeliveryOfficerId = request.RegionalDeliveryOfficer;
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}