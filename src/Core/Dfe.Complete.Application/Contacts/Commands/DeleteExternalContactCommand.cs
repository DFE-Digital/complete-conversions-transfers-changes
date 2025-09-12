using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Queries.QueryFilters;
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
    IContactReadRepository contactReadRepository,
    IContactWriteRepository contactWriteRepository,
    ILogger<DeleteExternalContactCommandHandler> logger
) : IRequestHandler<DeleteExternalContactCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var contactEntity = new ContactIdQuery(request.ContactId)
                .Apply(contactReadRepository.Contacts)
                .FirstOrDefault();

            if (contactEntity is null)
            {
                await unitOfWork.RollBackAsync();
                var message = ErrorMessagesConstants.NotFoundExternalContact.Replace("{Id}", request.ContactId.Value.ToString());
                return Result<bool>.Failure(message, ErrorType.NotFound);
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

            await contactWriteRepository.RemoveContactAsync(contactEntity, cancellationToken);

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
}