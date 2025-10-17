using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Queries.QueryFilters;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Contacts.Commands;

public record DeleteExternalContactCommand(ContactId ContactId) : IRequest<Result<bool>>;

public class DeleteExternalContactCommandHandler(
    IUnitOfWork unitOfWork,
    ICompleteRepository<Project> projectRepository,
    IContactReadRepository contactReadRepository,
    IContactWriteRepository contactWriteRepository,
    IKeyContactReadRepository keyContactReadRepository,
    IKeyContactWriteRepository keyContactWriteRepository,
    ILogger<DeleteExternalContactCommandHandler> logger
) : IRequestHandler<DeleteExternalContactCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {   
            await unitOfWork.BeginTransactionAsync();

            var contactEntity = await new ContactIdQuery(request.ContactId)
                .Apply(contactReadRepository.Contacts)
                .FirstOrDefaultAsync(cancellationToken);

            if (contactEntity is null)
            {
                await unitOfWork.RollBackAsync();
                var message = ErrorMessagesConstants.NotFoundExternalContact.Replace("{Id}", request.ContactId.Value.ToString());
                return Result<bool>.Failure(message, ErrorType.NotFound);
            }

            await UnsetProjectContactReferencesAsync(request.ContactId, cancellationToken);

            await contactWriteRepository.RemoveContactAsync(contactEntity, cancellationToken);

            await UnsetKeyContactReferencesAsync(request.ContactId, cancellationToken);

            await unitOfWork.CommitAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollBackAsync();

            var message = ErrorMessagesConstants.CouldNotDeleteExternalContact.Replace("{Id}", request.ContactId.Value.ToString());
            logger.LogError(ex, ErrorMessagesConstants.CouldNotDeleteExternalContact, request.ContactId);

            return Result<bool>.Failure(message);
        }
    }

    private async Task UnsetProjectContactReferencesAsync(ContactId contactId, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(x => x.IncomingTrustMainContactId == contactId
            || x.EstablishmentMainContactId == contactId || x.OutgoingTrustMainContactId == contactId
            || x.LocalAuthorityMainContactId == contactId, cancellationToken);

        if (project == null) return;

        if (project.EstablishmentMainContactId == contactId)
            project.EstablishmentMainContactId = null;

        if (project.IncomingTrustMainContactId == contactId)
            project.IncomingTrustMainContactId = null;

        if (project.OutgoingTrustMainContactId == contactId)
            project.OutgoingTrustMainContactId = null;

        if (project.LocalAuthorityMainContactId == contactId)
            project.LocalAuthorityMainContactId = null;

        await projectRepository.UpdateAsync(project, cancellationToken);
    }

    private async Task UnsetKeyContactReferencesAsync(ContactId contactId, CancellationToken cancellationToken)
    {
        var keycontact = await keyContactReadRepository.KeyContacts.Where(
            n => n.IncomingTrustCeoId == contactId
            || n.OutgoingTrustCeoId == contactId
            || n.HeadteacherId == contactId
            || n.ChairOfGovernorsId == contactId
            ).FirstOrDefaultAsync(cancellationToken);

        if (keycontact == null) return;

        if (keycontact.IncomingTrustCeoId == contactId)
            keycontact.IncomingTrustCeoId = null;

        if (keycontact.OutgoingTrustCeoId == contactId)
            keycontact.OutgoingTrustCeoId = null;

        if (keycontact.HeadteacherId == contactId)
            keycontact.HeadteacherId = null;

        if (keycontact.ChairOfGovernorsId == contactId)
            keycontact.ChairOfGovernorsId = null;

        await keyContactWriteRepository.UpdateKeyContactAsync(keycontact, cancellationToken);
    }
}