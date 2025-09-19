using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries.QueryFilters;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Contacts.Commands;

public record UpdateExternalContactCommand(ContactId ContactId, ContactDto contactDto) : IRequest<Result<bool>>;

public class UpdateExternalContactCommandHandler(
    IContactReadRepository contactReadRepository,
    IContactWriteRepository contactWriteRepository, 
    ISender sender,
    ILogger<UpdateExternalContactCommandHandler> logger
) : IRequestHandler<UpdateExternalContactCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateExternalContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var contactEntity = new ContactIdQuery(request.ContactId)
                .Apply(contactReadRepository.Contacts)
                .FirstOrDefault();
            
            if (contactEntity is null) return Result<bool>.Failure(ErrorMessagesConstants.NotFoundExternalContact.Replace("{Id}", request.ContactId.Value.ToString()), ErrorType.NotFound);

            var updateDto = request.contactDto;

            contactEntity.Name = updateDto.Name;
            contactEntity.Title = updateDto.Title;
            contactEntity.Phone = updateDto.Phone;
            contactEntity.Email = updateDto.Email;
            contactEntity.Category = updateDto.Category;
            contactEntity.OrganisationName = updateDto.OrganisationName;
            contactEntity.UpdatedAt = DateTime.UtcNow;

            await contactWriteRepository.UpdateContactAsync(contactEntity, cancellationToken);            

            await sender.Send(new UpdatePrimaryContactAtOrganisationCommand(contactEntity.ProjectId!, request.contactDto.PrimaryContact, contactEntity), cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            var message = ErrorMessagesConstants.CouldNotUpdateExternalContact.Replace("{Id}", request.contactDto.Id.Value.ToString());
            logger.LogError(ex, ErrorMessagesConstants.CouldNotUpdateExternalContact, request.contactDto.Id);

            return Result<bool>.Failure(message);
        }
    }
}