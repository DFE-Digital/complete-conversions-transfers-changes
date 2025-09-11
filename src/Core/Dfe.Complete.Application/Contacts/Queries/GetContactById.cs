using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetContactByIdQuery(ContactId ContactId) : IRequest<Result<ContactDto?>>;

    public class GetContactIdQueryHandler(
        ICompleteRepository<Contact> contactsRepository,
        IMapper mapper,
        ILogger<GetContactIdQueryHandler> logger)
        : IRequestHandler<GetContactByIdQuery, Result<ContactDto?>>
    {
        public async Task<Result<ContactDto?>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await contactsRepository.GetAsync(x => x.Id == request.ContactId);
                var contactDto = mapper.Map<ContactDto?>(result);
                return Result<ContactDto?>.Success(contactDto);
            }
            catch (Exception ex)
            {
                var message = ErrorMessagesConstants.ExceptionGettingExternalContact.Replace("{Id}", request.ContactId?.Value.ToString());
                logger.LogError(ex, ErrorMessagesConstants.ExceptionGettingExternalContact, request.ContactId);

                return Result<ContactDto?>.Failure(message);
            }
        }
    }
}