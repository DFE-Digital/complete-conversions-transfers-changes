using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.KeyContacts.Commands;

public record UpdateIncomingTrustCeoCommand(KeyContactId KeyContactId, ContactId IncomingTrustCeoId) : IRequest<Result<bool>>;

public class UpdateKeyContactIncomingTrustCeoCommandHandler(
    IKeyContactWriteRepository _keyContactWriteRepo,
    IKeyContactReadRepository _keyContactReadRepo,
    ILogger<UpdateKeyContactIncomingTrustCeoCommandHandler> logger
) : IRequestHandler<UpdateIncomingTrustCeoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateIncomingTrustCeoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keycontact = await _keyContactReadRepo.KeyContacts.Where(n => n.Id == request.KeyContactId).FirstOrDefaultAsync(cancellationToken);          

            if (keycontact is null) return Result<bool>.Failure($"KeyContact with ID {request.KeyContactId.Value} not found", ErrorType.NotFound);

            keycontact.IncomingTrustCeoId = request.IncomingTrustCeoId;

            await _keyContactWriteRepo.UpdateKeyContactAsync(keycontact, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateKeyContactIncomingTrustCeoCommandHandler), request);
            return Result<bool>.Failure($"Could not update keycontact  {request.KeyContactId.Value}: {ex.Message}");
        }
    }
}