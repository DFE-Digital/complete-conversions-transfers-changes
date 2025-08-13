using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdatePrimaryContactAtOrganisationCommand(ProjectId ProjectId, Contact Contact) : IRequest;

public class UpdatePrimaryContactAtOrganisation(ICompleteRepository<Project> projectRepository)
    : IRequestHandler<UpdatePrimaryContactAtOrganisationCommand>
{
    public async Task Handle(UpdatePrimaryContactAtOrganisationCommand request, CancellationToken cancellationToken)
    {
        
        if (request.Contact.ProjectId != request.ProjectId)
        {
            return;
        }
        
        var project = await projectRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project == null)
        {
            return;
        }

        switch (request.Contact.Category)
        {
            case ContactCategory.SchoolOrAcademy:
                project.EstablishmentMainContactId = request.Contact.Id;
                break;
            case ContactCategory.IncomingTrust:
                project.IncomingTrustMainContactId = request.Contact.Id;
                break;
            case ContactCategory.OutgoingTrust:
                project.OutgoingTrustMainContactId = request.Contact.Id;
                break;
            case ContactCategory.LocalAuthority:
                project.LocalAuthorityMainContactId = request.Contact.Id;
                break;
            case ContactCategory.Diocese:
            case ContactCategory.Other:
            case ContactCategory.Solicitor:
            default:
                return;    
        }
        
        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}