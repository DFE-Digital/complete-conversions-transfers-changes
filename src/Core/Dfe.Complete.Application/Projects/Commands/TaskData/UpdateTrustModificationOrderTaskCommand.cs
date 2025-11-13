using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateTrustModificationOrderTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Received,
        bool? Sent,
        bool? Cleared,
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateTrustModificationOrderTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateTrustModificationOrderTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateTrustModificationOrderTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            var notApplicableSelected = request.NotApplicable == true;

            tasksData.TrustModificationOrderNotApplicable = request.NotApplicable;
            tasksData.TrustModificationOrderReceived = notApplicableSelected ? null : request.Received;
            tasksData.TrustModificationOrderSentLegal = notApplicableSelected ? null : request.Sent;
            tasksData.TrustModificationOrderCleared = notApplicableSelected ? null : request.Cleared;
            tasksData.TrustModificationOrderSaved = notApplicableSelected ? null : request.Saved;


            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
