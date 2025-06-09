using Dfe.Complete.Application.Projects.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateRegionalDeliveryOfficerCommand(
    Urn ProjectUrn,
    UserId RegionalDeliveryOfficer
    ) : IRequest;

public class UpdateRegionalDeliveryOfficer(
    ICompleteRepository<Project> projectRepository,
    ICreateProjectCommon createProjectCommon)
    : IRequestHandler<UpdateRegionalDeliveryOfficerCommand>
{
    public async Task Handle(UpdateRegionalDeliveryOfficerCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Urn == request.ProjectUrn, cancellationToken);
        project.RegionalDeliveryOfficerId = request.RegionalDeliveryOfficer;
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}