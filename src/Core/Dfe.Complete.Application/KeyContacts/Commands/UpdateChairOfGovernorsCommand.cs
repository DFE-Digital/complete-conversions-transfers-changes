using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.KeyContacts.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.KeyContacts.Commands;

public record UpdateChairOfGovernorsCommand(KeyContactId KeyContactId, ContactId ChairOfGovernorsId) : IRequest<Result<bool>>;

internal class UpdateChairOfGovernorsCommandHandler(
    IKeyContactWriteRepository _keyContactWriteRepo,
    IKeyContactReadRepository _keyContactReadRepo    
) : IRequestHandler<UpdateChairOfGovernorsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateChairOfGovernorsCommand request, CancellationToken cancellationToken)
    {   
        var keycontact = await _keyContactReadRepo.KeyContacts.FirstOrDefaultAsync(n => n.Id == request.KeyContactId, cancellationToken)
            ?? throw new NotFoundException($"KeyContact with {request.KeyContactId} not found.");

        keycontact.ChairOfGovernorsId = request.ChairOfGovernorsId;

        await _keyContactWriteRepo.UpdateKeyContactAsync(keycontact, cancellationToken);

        return Result<bool>.Success(true);  
    }
}