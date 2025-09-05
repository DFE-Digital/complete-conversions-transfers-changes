using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdatePrimaryContactAtOrganisationCommand(ProjectId ProjectId, bool PrimaryContact, Contact Contact) : IRequest;

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

                if (project.IncomingTrustMainContactId == request.Contact.Id)
                {
                    project.IncomingTrustMainContactId = null;
                }
                if (project.OutgoingTrustMainContactId == request.Contact.Id)
                {
                    project.OutgoingTrustMainContactId = null;
                }
                if (project.LocalAuthorityMainContactId == request.Contact.Id)
                {
                    project.LocalAuthorityMainContactId = null;
                }

                if (project.EstablishmentMainContactId == request.Contact.Id && !request.PrimaryContact)
                {
                    project.EstablishmentMainContactId = null;
                }
                else
                {
                    if (request.PrimaryContact)
                    {
                        project.EstablishmentMainContactId = request.Contact.Id;
                    }
                    else
                    {
                        return;
                    }
                }
                
                break;
            case ContactCategory.IncomingTrust:

                if (project.EstablishmentMainContactId == request.Contact.Id)
                {
                    project.EstablishmentMainContactId = null;
                }
                if (project.OutgoingTrustMainContactId == request.Contact.Id)
                {
                    project.OutgoingTrustMainContactId = null;
                }
                if (project.LocalAuthorityMainContactId == request.Contact.Id)
                {
                    project.LocalAuthorityMainContactId = null;
                }

                if (project.IncomingTrustMainContactId == request.Contact.Id && !request.PrimaryContact)
                {
                    project.IncomingTrustMainContactId = null;
                }
                else
                {
                    if (request.PrimaryContact)
                    {
                        project.IncomingTrustMainContactId = request.Contact.Id;
                    }
                    else
                    {
                        return;
                    }
                }
                
                break;
            case ContactCategory.OutgoingTrust:

                if (project.EstablishmentMainContactId == request.Contact.Id)
                {
                    project.EstablishmentMainContactId = null;
                }
                if (project.IncomingTrustMainContactId == request.Contact.Id)
                {
                    project.IncomingTrustMainContactId = null;
                }
                if (project.LocalAuthorityMainContactId == request.Contact.Id)
                {
                    project.LocalAuthorityMainContactId = null;
                }

                if (project.OutgoingTrustMainContactId == request.Contact.Id && !request.PrimaryContact)
                {
                    project.OutgoingTrustMainContactId = null;
                }
                else
                {
                    if (request.PrimaryContact)
                    {
                        project.OutgoingTrustMainContactId = request.Contact.Id;
                    }
                    else
                    {
                        return;
                    }
                }                
               
                break;
            case ContactCategory.LocalAuthority:

                if (project.EstablishmentMainContactId == request.Contact.Id)
                {
                    project.EstablishmentMainContactId = null;
                }
                if (project.IncomingTrustMainContactId == request.Contact.Id)
                {
                    project.IncomingTrustMainContactId = null;
                }
                if (project.OutgoingTrustMainContactId == request.Contact.Id)
                {
                    project.OutgoingTrustMainContactId = null;
                }

                if (project.LocalAuthorityMainContactId == request.Contact.Id && !request.PrimaryContact)
                {
                    project.LocalAuthorityMainContactId = null;
                }
                else
                {
                    if (request.PrimaryContact)
                    {
                        project.LocalAuthorityMainContactId = request.Contact.Id;
                    }
                    else
                    {
                        return;
                    }
                }
                
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