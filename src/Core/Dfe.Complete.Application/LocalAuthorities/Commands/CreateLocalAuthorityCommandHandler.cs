using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.LocalAuthorities.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.LocalAuthorities.Commands
{
    public record CreateLocalAuthorityCommand(
       string Code,
       string Name,
       string Address1,
       string? Address2,
       string? Address3,
       string? AddressTown,
       string? AddressCounty,
       string AddressPostcode,
       string? Title,
       string? ContactName,
       string? Email,
       string? Phone) : IRequest<Result<CreateLocalAuthorityDto?>>;

    public class CreateLocalAuthorityCommandHandler(
       IUnitOfWork unitOfWork,
       ICompleteRepository<LocalAuthority> localAuthorityRepository,
       ICompleteRepository<Contact> contactRepository,
       ILogger<CreateLocalAuthorityCommandHandler> logger) : IRequestHandler<CreateLocalAuthorityCommand, Result<CreateLocalAuthorityDto?>>
    {
        public async Task<Result<CreateLocalAuthorityDto?>> Handle(CreateLocalAuthorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var localAuthorityId = new LocalAuthorityId(Guid.NewGuid());
                ContactId? contactId = null;
                await unitOfWork.BeginTransactionAsync();
                var hasLocalAuthority = await localAuthorityRepository.ExistsAsync(x => x.Code == request.Code, cancellationToken);
                if (hasLocalAuthority)
                {
                    throw new AlreadyExistsException(string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, request.Code));
                }
                var localAuthority = LocalAuthority.Create(localAuthorityId, request.Name, request.Code, new AddressDetails(request.Address1,
                    request.Address2, request.Address3, request.AddressTown, request.AddressCounty,
                    request.AddressPostcode), DateTime.UtcNow);

                await localAuthorityRepository.AddAsync(localAuthority, cancellationToken);
                if (!string.IsNullOrWhiteSpace(request.Title) && !string.IsNullOrWhiteSpace(request.ContactName))
                {
                    contactId = new ContactId(Guid.NewGuid());
                    var contact = Contact.Create(contactId, request.Title, request.ContactName, request.Email, request.Phone, localAuthority.Id, DateTime.Now);
                    await contactRepository.AddAsync(contact, cancellationToken);
                }
                await unitOfWork.CommitAsync();

                return Result<CreateLocalAuthorityDto?>.Success(new CreateLocalAuthorityDto(localAuthorityId, contactId));
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex, ErrorMessagesConstants.ExceptionWhileCreatingLocalAuthority, request.Code);

                return Result<CreateLocalAuthorityDto?>.Failure(ex.Message);
            }
        }
    }
}
