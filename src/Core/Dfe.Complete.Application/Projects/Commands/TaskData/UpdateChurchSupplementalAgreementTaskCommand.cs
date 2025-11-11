using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateChurchSupplementalAgreementTaskCommand(
        TaskDataId TaskDataId,
        [Required] ProjectType? ProjectType,
        bool? NotApplicable,
        bool? Received,
        bool? Cleared,
        bool? Signed,
        bool? SignedByDiocese,
        bool? Saved,
        bool? SignedBySecretaryState,
        bool? SentOrSaved
    ) : IRequest<Result<bool>>;

    internal class UpdateChurchSupplementalAgreementTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateChurchSupplementalAgreementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateChurchSupplementalAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            if (request.ProjectType == ProjectType.Conversion)
            {
                await UpdateConversionTaskDataAsync(request.TaskDataId, request, cancellationToken);
            }
            else if (request.ProjectType == ProjectType.Transfer)
            {
                await UpdateTransferTaskDataAsync(request.TaskDataId, request, cancellationToken);
            }

            return Result<bool>.Success(true);
        }


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateChurchSupplementalAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.ChurchSupplementalAgreementNotApplicable = request.NotApplicable;
            tasksData.ChurchSupplementalAgreementReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.ChurchSupplementalAgreementCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.ChurchSupplementalAgreementSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.ChurchSupplementalAgreementSignedDiocese = request.NotApplicable == true ? null : request.SignedByDiocese;
            tasksData.ChurchSupplementalAgreementSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.ChurchSupplementalAgreementSent = request.NotApplicable == true ? null : request.SentOrSaved;
            tasksData.ChurchSupplementalAgreementSignedSecretaryState = request.NotApplicable == true ? null : request.SignedBySecretaryState;


            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateChurchSupplementalAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.ChurchSupplementalAgreementNotApplicable = request.NotApplicable;
            tasksData.ChurchSupplementalAgreementReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.ChurchSupplementalAgreementCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.ChurchSupplementalAgreementSignedIncomingTrust = request.NotApplicable == true ? null : request.Signed;
            tasksData.ChurchSupplementalAgreementSignedDiocese = request.NotApplicable == true ? null : request.SignedByDiocese;
            tasksData.ChurchSupplementalAgreementSavedAfterSigningByTrustDiocese = request.NotApplicable == true ? null : request.Saved;
            tasksData.ChurchSupplementalAgreementSignedSecretaryState = request.NotApplicable == true ? null : request.SignedBySecretaryState;
            tasksData.ChurchSupplementalAgreementSavedAfterSigningBySecretaryState = request.NotApplicable == true ? null : request.SentOrSaved;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }
    }
}
