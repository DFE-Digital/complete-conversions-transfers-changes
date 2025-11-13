using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Contacts.Interfaces;
using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries.QueryFilters;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Contacts.Queries
{
    public record GetContactByIdQuery(ContactId ContactId) : IRequest<Result<ContactDto?>>;

    public class GetContactIdQueryHandler(
        IContactReadRepository contactReadRepository,
        IMapper mapper,
        ILogger<GetContactIdQueryHandler> logger)
        : IRequestHandler<GetContactByIdQuery, Result<ContactDto?>>
    {
        public async Task<Result<ContactDto?>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var contact = await new ContactIdQuery(request.ContactId)
                    .Apply(contactReadRepository.Contacts.AsNoTracking())
                    .FirstOrDefaultAsync(cancellationToken);

                var contactDto = mapper.Map<ContactDto?>(contact);
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