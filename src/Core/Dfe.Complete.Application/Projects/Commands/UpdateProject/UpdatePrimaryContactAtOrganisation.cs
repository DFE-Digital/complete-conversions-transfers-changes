using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

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
        
        var project = await projectRepository.FindAsync(request.ProjectId, cancellationToken);

        if (project == null)
        {
            return;
        }

        // Use local variables for ref assignment, then update the properties after
        ContactId? establishmentMainContactId = project.EstablishmentMainContactId;
        ContactId? incomingTrustMainContactId = project.IncomingTrustMainContactId;
        ContactId? outgoingTrustMainContactId = project.OutgoingTrustMainContactId;
        ContactId? localAuthorityMainContactId = project.LocalAuthorityMainContactId;

        switch (request.Contact.Category)
        {
            case ContactCategory.SchoolOrAcademy:
                ClearIfMatching(ref incomingTrustMainContactId, request.Contact.Id);
                ClearIfMatching(ref outgoingTrustMainContactId, request.Contact.Id);
                ClearIfMatching(ref localAuthorityMainContactId, request.Contact.Id);

                UpdateMainContactId(ref establishmentMainContactId, request.Contact, request.PrimaryContact);
                break;

            case ContactCategory.IncomingTrust:
                ClearIfMatching(ref establishmentMainContactId, request.Contact.Id);
                ClearIfMatching(ref outgoingTrustMainContactId, request.Contact.Id);
                ClearIfMatching(ref localAuthorityMainContactId, request.Contact.Id);

                UpdateMainContactId(ref incomingTrustMainContactId, request.Contact, request.PrimaryContact);
                break;

            case ContactCategory.OutgoingTrust:
                ClearIfMatching(ref establishmentMainContactId, request.Contact.Id);
                ClearIfMatching(ref incomingTrustMainContactId, request.Contact.Id);
                ClearIfMatching(ref localAuthorityMainContactId, request.Contact.Id);

                UpdateMainContactId(ref outgoingTrustMainContactId, request.Contact, request.PrimaryContact);
                break;

            case ContactCategory.LocalAuthority:
                ClearIfMatching(ref establishmentMainContactId, request.Contact.Id);
                ClearIfMatching(ref incomingTrustMainContactId, request.Contact.Id);
                ClearIfMatching(ref outgoingTrustMainContactId, request.Contact.Id);

                UpdateMainContactId(ref localAuthorityMainContactId, request.Contact, request.PrimaryContact);
                break;

            case ContactCategory.Diocese:
            case ContactCategory.Other:
            case ContactCategory.Solicitor:
            default:
                return;
        }

        // Assign the updated local variables back to the properties
        project.EstablishmentMainContactId = establishmentMainContactId;
        project.IncomingTrustMainContactId = incomingTrustMainContactId;
        project.OutgoingTrustMainContactId = outgoingTrustMainContactId;
        project.LocalAuthorityMainContactId = localAuthorityMainContactId;

        await projectRepository.UpdateAsync(project, cancellationToken);
    }

    static void ClearIfMatching(ref ContactId? field, ContactId contactId)
    {
        if (field?.Value == contactId.Value)
        {
            field = null;
        }
    }

    void UpdateMainContactId(ref ContactId? mainContactId, Contact requestContact, bool isPrimary)
    {
        if (mainContactId?.Value == requestContact.Id.Value && !isPrimary)
        {
            mainContactId = null;
        }
        else if (isPrimary)
        {
            mainContactId = requestContact.Id;
        }
    }
}