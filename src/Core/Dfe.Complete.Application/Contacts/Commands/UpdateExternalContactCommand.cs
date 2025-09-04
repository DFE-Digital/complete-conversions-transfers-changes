using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Contacts.Commands;

public record UpdateExternalContactCommand(ContactId ContactId, ContactDto contactDto) : IRequest<Result<ContactDto>>;

public class UpdateExternalContactCommandHandler(
    ICompleteRepository<Contact> contactsRepository,
    IMapper mapper,
    ILogger<UpdateExternalContactCommandHandler> logger
) : IRequestHandler<UpdateExternalContactCommand, Result<ContactDto>>
{
    public async Task<Result<ContactDto?>> Handle(UpdateExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contactEntity = await contactsRepository.GetAsync(x => x.Id == request.ContactId);
            if (contactEntity is null) return Result<ContactDto?>.Failure(string.Format(ErrorMessagesConstants.NotFoundExternalContact, request.ContactId.Value), ErrorType.NotFound);

            var updateDto = request.contactDto;

            contactEntity.Name = updateDto.Name;
            contactEntity.Title = updateDto.Title;  
            contactEntity.Phone = updateDto.Phone;
            contactEntity.Email = updateDto.Email;
            contactEntity.Category = updateDto.Category;
            contactEntity.OrganisationName = updateDto.OrganisationName;
            contactEntity.UpdatedAt = DateTime.UtcNow;

            var savedEntity = await contactsRepository.UpdateAsync(contactEntity, cancellationToken);

            var result = mapper.Map<ContactDto?>(savedEntity);
            return Result<ContactDto?>.Success(result);
        }
        catch (Exception ex)
        {
            var message = string.Format(ErrorMessagesConstants.CouldNotUpdateExternalContact, request.contactDto.Id.Value);
            logger.LogError(ex, message);            
            return Result<ContactDto?>.Failure(message);
        }
    }
}