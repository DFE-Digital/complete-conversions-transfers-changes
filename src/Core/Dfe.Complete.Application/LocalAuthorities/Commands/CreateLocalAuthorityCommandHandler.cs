using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.LocalAuthorities.Commands
{
    public record CreateLocalAuthorityCommand(LocalAuthorityId Id,
       string Code,
       string Name,
       string Address1,
       string? Address2,
       string? Address3,
       string? AddressTown,
       string? AddressCounty,
       string AddressPostcode,
       ContactId ContactId,
       string Title,
       string ContactName,
       string? Email,
       string? Phone) : IRequest<Result<LocalAuthorityId?>>;

    public class CreateLocalAuthorityCommandHandler(
       IUnitOfWork unitOfWork,
       ICompleteRepository<LocalAuthority> localAuthorityRepository,
       ICompleteRepository<Contact> contactRepository,
       ILogger<CreateLocalAuthorityCommandHandler> logger) : IRequestHandler<CreateLocalAuthorityCommand, Result<LocalAuthorityId?>>
    {
        public async Task<Result<LocalAuthorityId?>> Handle(CreateLocalAuthorityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync();
                var localAuthority = await localAuthorityRepository.FindAsync(x => x.Name == request.Name && x.Code == request.Code, cancellationToken);
                if (localAuthority != null)
                {
                    logger.LogWarning("Cannot create Local authority as it is already existed. Request: {Request}", request);
                    throw new Exception($"Cannot create Local authority with code {request.Code} as it is already existed.");
                } 
                localAuthority = LocalAuthority.CreateLocalAuthority(request.Id, request.Name, request.Code, request.Address1,
                    request.Address2, request.Address3, request.AddressTown, request.AddressCounty,
                    request.AddressPostcode, DateTime.UtcNow);

                await localAuthorityRepository.AddAsync(localAuthority, cancellationToken);
                if (!string.IsNullOrWhiteSpace(request.Title) && !string.IsNullOrWhiteSpace(request.ContactName))
                {
                    var contact = Contact.CreateLocalAuthorityContact(request.ContactId, request.Title, request.Name, request.Email, request.Phone, localAuthority.Id, DateTime.Now);
                    await contactRepository.AddAsync(contact, cancellationToken);
                }
                await unitOfWork.CommitAsync();

                return Result<LocalAuthorityId?>.Success(localAuthority.Id);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex, "Error occurred while creating LocalAuthority with code {Code}.", request.Code);

                return Result<LocalAuthorityId?>.Failure(ex.Message);
            }
        }
    }
}
