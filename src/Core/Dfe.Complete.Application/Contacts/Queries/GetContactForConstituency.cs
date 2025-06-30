using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using Dfe.PersonsApi.Client.Contracts;
using MediatR;

namespace Dfe.Complete.Application.Contacts.Queries;

public record GetContactForConstituencyQuery(string Constituency) : IRequest<Result<Contact>>;

public class GetContactForConstituency(IConstituenciesClient client)
    : IRequestHandler<GetContactForConstituencyQuery, Result<Contact>>
{
    public async Task<Result<Contact>> Handle(GetContactForConstituencyQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await client.GetMemberOfParliamentByConstituencyAsync(request.Constituency, cancellationToken);
            if (result is null) throw new NotFoundException($"No MP found for constituency {request.Constituency}");
            var contact = new Contact()
            {
                Name = result.DisplayName,
                Category = ContactCategory.Other,
                Email = result.Email,
                Phone = result.Phone
            };
            return Result<Contact>.Success(contact);
        }
        catch (Exception e)
        {
            return Result<Contact>.Failure(e.Message);
        }
        
    }
}