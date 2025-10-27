using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.KeyContacts.Commands;

public record UpdateChairOfGovernorsCommand(KeyContactId KeyContactId, ContactId ChairOfGovernorsId) : IRequest<Result<bool>>;

internal class UpdateChairOfGovernorsCommandHandler(
    IKeyContactWriteRepository _keyContactWriteRepo,
    IKeyContactReadRepository _keyContactReadRepo,
    ILogger<UpdateChairOfGovernorsCommandHandler> logger
) : IRequestHandler<UpdateChairOfGovernorsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateChairOfGovernorsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keycontact = await _keyContactReadRepo.KeyContacts.Where(n => n.Id == request.KeyContactId).FirstOrDefaultAsync(cancellationToken);          

            if (keycontact is null) return Result<bool>.Failure($"KeyContact with ID {request.KeyContactId.Value} not found", ErrorType.NotFound);

            keycontact.ChairOfGovernorsId = request.ChairOfGovernorsId;

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