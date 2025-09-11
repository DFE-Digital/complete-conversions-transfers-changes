using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Contacts.Commands;

public record DeleteExternalContactCommand(ContactId ContactId) : IRequest<Result<bool>>;

public class DeleteExternalContactCommandHandler(
    IUnitOfWork unitOfWork,
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<Contact> contactRepository,    
    ILogger<DeleteExternalContactCommandHandler> logger
) : IRequestHandler<DeleteExternalContactCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var contactEntity = await contactRepository.FindAsync(request.ContactId, cancellationToken);
            
            if (contactEntity is null)
            {
                await unitOfWork.RollBackAsync();
                return Result<bool>.Failure(string.Format(ErrorMessagesConstants.NotFoundExternalContact, request.ContactId.Value), ErrorType.NotFound);
            }

            var project = await projectRepository.FindAsync(x => x.IncomingTrustMainContactId == request.ContactId
           || x.EstablishmentMainContactId == request.ContactId || x.OutgoingTrustMainContactId == request.ContactId
           || x.LocalAuthorityMainContactId == request.ContactId, cancellationToken);

            if (project != null)
            {
                if (project.EstablishmentMainContactId == request.ContactId)
                {
                    project.EstablishmentMainContactId = null;
                }

                if (project.IncomingTrustMainContactId == request.ContactId)
                {
                    project.IncomingTrustMainContactId = null;
                }

                if (project.OutgoingTrustMainContactId == request.ContactId)
                {
                    project.OutgoingTrustMainContactId = null;
                }

                if (project.LocalAuthorityMainContactId == request.ContactId)
                {
                    project.LocalAuthorityMainContactId = null;
                }

                await projectRepository.UpdateAsync(project, cancellationToken);
            }

            await contactRepository.RemoveAsync(contactEntity, cancellationToken);

            await unitOfWork.CommitAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollBackAsync();
            var message = string.Format(ErrorMessagesConstants.CouldNotDeleteExternalContact, request.ContactId);
            logger.LogError(ex, "{message}", message);
            return Result<bool>.Failure(message);
        }
    }
}