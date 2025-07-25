﻿using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
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
       ContactId? ContactId,
       string? Title,
       string? ContactName,
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
                var hasLocalAuthority = await localAuthorityRepository.ExistsAsync(x => x.Code == request.Code, cancellationToken);
                if (hasLocalAuthority)
                {
                    throw new AlreadyExistsException(string.Format(ErrorMessagesConstants.AlreadyExistedLocalAuthorityWithCode, request.Code));
                } 
                var localAuthority = LocalAuthority.Create(request.Id, request.Name, request.Code, new AddressDetails(request.Address1,
                    request.Address2, request.Address3, request.AddressTown, request.AddressCounty,
                    request.AddressPostcode), DateTime.UtcNow);

                await localAuthorityRepository.AddAsync(localAuthority, cancellationToken);
                if (!string.IsNullOrWhiteSpace(request.Title) && !string.IsNullOrWhiteSpace(request.ContactName))
                {
                    var contact = Contact.Create(request.ContactId!, request.Title, request.ContactName, request.Email, request.Phone, localAuthority.Id, DateTime.Now);
                    await contactRepository.AddAsync(contact, cancellationToken);
                }
                await unitOfWork.CommitAsync();

                return Result<LocalAuthorityId?>.Success(localAuthority.Id);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollBackAsync();
                logger.LogError(ex, ErrorMessagesConstants.ExceptionWhileCreatingLocalAuthority, request.Code);

                return Result<LocalAuthorityId?>.Failure(ex.Message);
            }
        }
    }
}
