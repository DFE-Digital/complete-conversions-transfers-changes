using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateDeedOfNovationAndVariationTaskCommand(
        TaskDataId TaskDataId,
        bool? Received,
        bool? Cleared,
        bool? SignedOutgoingTrust,
        bool? SignedIncomingTrust,
        bool? Saved,
        bool? SignedSecretaryState,
        bool? SavedAfterSign) : IRequest<Result<bool>>;
    public class UpdateDeedOfNovationAndVariationTaskCommandHandler(
       ITaskDataReadRepository taskDataReadRepository,
       ITaskDataWriteRepository taskDataWriteRepository)
       : IRequestHandler<UpdateDeedOfNovationAndVariationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDeedOfNovationAndVariationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                 ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.DeedOfNovationAndVariationSaved = request.Saved;
            tasksData.DeedOfNovationAndVariationCleared = request.Cleared;
            tasksData.DeedOfNovationAndVariationReceived = request.Received;
            tasksData.DeedOfNovationAndVariationSignedIncomingTrust = request.SignedIncomingTrust;
            tasksData.DeedOfNovationAndVariationSignedOutgoingTrust = request.SignedOutgoingTrust;
            tasksData.DeedOfNovationAndVariationSignedSecretaryState = request.SignedSecretaryState;
            tasksData.DeedOfNovationAndVariationSaveAfterSign = request.SavedAfterSign;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
