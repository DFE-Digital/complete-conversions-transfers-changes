using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateDeedTerminationChurchSupplementalAgreementTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Received,
        bool? Cleared,
        bool? Signed,
        bool? SignedByDiocese,
        bool? Saved,
        bool? SignedBySecretaryState,
        bool? SavedAfterSigningBySecretaryState
    ) : IRequest<Result<bool>>;

    internal class UpdateDeedTerminationChurchSupplementalAgreementTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateDeedTerminationChurchSupplementalAgreementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDeedTerminationChurchSupplementalAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.DeedTerminationChurchAgreementNotApplicable = request.NotApplicable;
            tasksData.DeedTerminationChurchAgreementReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.DeedTerminationChurchAgreementCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.DeedTerminationChurchAgreementSignedOutgoingTrust = request.NotApplicable == true ? null : request.Signed;
            tasksData.DeedTerminationChurchAgreementSignedDiocese = request.NotApplicable == true ? null : request.SignedByDiocese;
            tasksData.DeedTerminationChurchAgreementSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.DeedTerminationChurchAgreementSignedSecretaryState = request.NotApplicable == true ? null : request.SignedBySecretaryState;
            tasksData.DeedTerminationChurchAgreementSavedAfterSigningBySecretaryState = request.NotApplicable == true ? null : request.SavedAfterSigningBySecretaryState;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
