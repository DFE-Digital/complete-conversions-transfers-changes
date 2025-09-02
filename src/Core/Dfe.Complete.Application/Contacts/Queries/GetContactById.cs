using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects; 
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetContactByIdQuery(ContactId ContactId) : IRequest<Result<Contact?>>;

    public class GetContactIdQueryHandler(ICompleteRepository<Contact> contactsRepository, ILogger<GetContactIdQueryHandler> logger)
        : IRequestHandler<GetContactByIdQuery, Result<Contact?>>
    {
        public async Task<Result<Contact?>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await contactsRepository.GetAsync(x => x.Id == request.ContactId);                
                return Result<Contact?>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {ContactId} Request - {@Request}", nameof(GetContactIdQueryHandler), request);
                return Result<Contact?>.Failure(ex.Message);
            }
        }
    }
}