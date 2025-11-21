using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.KeyContacts.Commands;

public record UpdateOutgoingTrustCeoCommand(KeyContactId KeyContactId, ContactId OutgoingTrustCeoId) : IRequest<Result<bool>>;

internal class UpdateKeyContactOutgoingTrustCeoCommandHandler(
    IKeyContactWriteRepository _keyContactWriteRepo,
    IKeyContactReadRepository _keyContactReadRepo,
    ILogger<UpdateKeyContactOutgoingTrustCeoCommandHandler> logger
) : IRequestHandler<UpdateOutgoingTrustCeoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOutgoingTrustCeoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keycontact = await _keyContactReadRepo.KeyContacts.Where(n => n.Id == request.KeyContactId).FirstOrDefaultAsync(cancellationToken);

            if (keycontact is null) return Result<bool>.Failure($"KeyContact with ID {request.KeyContactId.Value} not found", ErrorType.NotFound);

            keycontact.OutgoingTrustCeoId = request.OutgoingTrustCeoId;

            await _keyContactWriteRepo.UpdateKeyContactAsync(keycontact, cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(UpdateKeyContactOutgoingTrustCeoCommandHandler), request);
            return Result<bool>.Failure($"Could not update keycontact  {request.KeyContactId.Value}: {ex.Message}");
        }
    }
}