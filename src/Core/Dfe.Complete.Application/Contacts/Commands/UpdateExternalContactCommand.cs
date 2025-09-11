using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Contacts.Commands;

public record UpdateExternalContactCommand(ContactId ContactId, ContactDto contactDto) : IRequest<Result<ContactDto>>;

public class UpdateExternalContactCommandHandler(
    ICompleteRepository<Contact> contactRepository,
    IMapper mapper,
    ISender sender,
    ILogger<UpdateExternalContactCommandHandler> logger
) : IRequestHandler<UpdateExternalContactCommand, Result<ContactDto>>
{
    public async Task<Result<ContactDto>> Handle(UpdateExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contactEntity = await contactRepository.GetAsync(x => x.Id == request.ContactId);
            if (contactEntity is null) return Result<ContactDto>.Failure(ErrorMessagesConstants.NotFoundExternalContact.Replace("{Id}", request.ContactId.Value.ToString()), ErrorType.NotFound);

            var updateDto = request.contactDto;

            contactEntity.Name = updateDto.Name;
            contactEntity.Title = updateDto.Title;
            contactEntity.Phone = updateDto.Phone;
            contactEntity.Email = updateDto.Email;
            contactEntity.Category = updateDto.Category;
            contactEntity.OrganisationName = updateDto.OrganisationName;
            contactEntity.UpdatedAt = DateTime.UtcNow;

            var savedEntity = await contactRepository.UpdateAsync(contactEntity, cancellationToken);

            var result = mapper.Map<ContactDto>(savedEntity);

            await sender.Send(new UpdatePrimaryContactAtOrganisationCommand(savedEntity.ProjectId!, request.contactDto.PrimaryContact, savedEntity), cancellationToken);

            return Result<ContactDto>.Success(result);
        }
        catch (Exception ex)
        {
            var message = ErrorMessagesConstants.CouldNotUpdateExternalContact.Replace("{Id}", request.contactDto.Id.Value.ToString());
            logger.LogError(ex, ErrorMessagesConstants.CouldNotUpdateExternalContact, request.contactDto.Id);

            return Result<ContactDto>.Failure(message);
        }
    }
}