using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmIncomingTrustHasCompleteAllActionsTaskCommand(TaskDataId TaskDataId, 
        bool? Emailed,
        bool? Saved
        ) : IRequest<Result<bool>>;

    internal class UpdateConfirmIncomingTrustHasCompleteAllActionsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmIncomingTrustHasCompleteAllActionsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmIncomingTrustHasCompleteAllActionsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.ConfirmIncomingTrustHasCompletedAllActionsEmailed = request.Emailed;
            tasksData.ConfirmIncomingTrustHasCompletedAllActionsSaved = request.Saved;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
