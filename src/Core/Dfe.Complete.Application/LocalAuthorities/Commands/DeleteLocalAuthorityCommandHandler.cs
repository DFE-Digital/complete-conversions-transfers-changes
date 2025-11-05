using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.LocalAuthorities.Commands
{
    public record DeleteLocalAuthorityCommand(LocalAuthorityId Id, ContactId? ContactId) : IRequest<Result<bool>>;

    public class DeleteLocalAuthorityCommandHandler(
        IUnitOfWork unitOfWork,
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<LocalAuthority> localAuthorityRepository,
        ICompleteRepository<Contact> contactRepository,
        ILogger<DeleteLocalAuthorityCommandHandler> logger) : IRequestHandler<DeleteLocalAuthorityCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteLocalAuthorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var project = await projectRepository.FindAsync(x => x.LocalAuthorityId == request.Id, cancellationToken);
                if (project != null)
                {
                    throw new DependencyException(ErrorMessagesConstants.CannotDeleteLocalAuthorityAsLinkedToProject);
                }

                var localAuthority = await localAuthorityRepository.FindAsync(request.Id, cancellationToken) ?? throw new NotFoundException(string.Format(ErrorMessagesConstants.NotFoundLocalAuthority, request.Id));
                await localAuthorityRepository.RemoveAsync(localAuthority, cancellationToken);

                if (request.ContactId != null)
                {
                    var contact = await contactRepository.FindAsync(x => x.Id == request.ContactId && x.LocalAuthorityId == request.Id, cancellationToken);
                    if (contact != null)
                    {
                        await contactRepository.RemoveAsync(contact, cancellationToken);
                    }
                }

                await unitOfWork.CommitAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex, ErrorMessagesConstants.ExceptionWhileDeletingLocalAuthority, request.Id);
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
