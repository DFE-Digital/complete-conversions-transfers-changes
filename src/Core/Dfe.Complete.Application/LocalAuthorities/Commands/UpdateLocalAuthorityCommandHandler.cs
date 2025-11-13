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
    public record UpdateLocalAuthorityCommand(LocalAuthorityId Id,
       string Code,
       string Address1,
       string? Address2,
       string? Address3,
       string? AddressTown,
       string? AddressCounty,
       string AddressPostcode,
       ContactId? ContactId = null,
       string? Title = null,
       string? ContactName = null,
       string? Email = null,
       string? Phone = null) : IRequest<Result<bool>>;

    public class UpdateLocalAuthorityCommandHandler(
        IUnitOfWork unitOfWork,
        ICompleteRepository<LocalAuthority> localAuthorityRepository,
        ICompleteRepository<Contact> contactRepository,
        ILogger<UpdateLocalAuthorityCommandHandler> logger) : IRequestHandler<UpdateLocalAuthorityCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateLocalAuthorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var localAuthority = await localAuthorityRepository.FindAsync(x => x.Id == request.Id, cancellationToken) ?? throw new NotFoundException(ErrorMessagesConstants.CannotUpdateLocalAuthorityAsNotExisted);
                if (localAuthority.Code != request.Code)
                {
                    var hasLocalAuthorityWithSameCode = await localAuthorityRepository.ExistsAsync(x => x.Code == request.Code, cancellationToken);
                    if (hasLocalAuthorityWithSameCode)
                    {
                        throw new AlreadyExistsException(string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, request.Code));
                    }
                }
                localAuthority.Update(request.Code, new AddressDetails(request.Address1,
                    request.Address2, request.Address3, request.AddressTown, request.AddressCounty,
                    request.AddressPostcode), DateTime.UtcNow);
                await localAuthorityRepository.UpdateAsync(localAuthority, cancellationToken);

                if (!string.IsNullOrWhiteSpace(request.Title) && !string.IsNullOrWhiteSpace(request.ContactName))
                {
                    var contact = await contactRepository.FindAsync(x => x.Id == request.ContactId && x.LocalAuthorityId == request.Id, cancellationToken);
                    if (contact == null)
                    {
                        contact = Contact.Create(request.ContactId!, request.Title, request.ContactName!, request.Email, request.Phone, localAuthority.Id, DateTime.Now);
                        await contactRepository.AddAsync(contact, cancellationToken);
                    }
                    else
                    {
                        contact.Update(request.Title!, request.ContactName!, request.Email, request.Phone, DateTime.Now);
                        await contactRepository.UpdateAsync(contact, cancellationToken);
                    }
                }
                await unitOfWork.CommitAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex, message: ErrorMessagesConstants.ExceptionWhileUpdatingLocalAuthority, request.Id);

                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
