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
    public record UpdateSupplementalFundingAgreementTaskCommand(
        TaskDataId TaskDataId,
        [Required] ProjectType? ProjectType,
        bool? Received,
        bool? Cleared,
        bool? Sent,
        bool? Saved,
        bool? Signed,
        bool? SignedSecretaryState) : IRequest<Result<bool>>;

    internal class UpdateSupplementalFundingAgreementTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateSupplementalFundingAgreementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateSupplementalFundingAgreementTaskCommand request, CancellationToken cancellationToken)
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

        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateSupplementalFundingAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.SupplementalFundingAgreementSaved = request.Saved;
            tasksData.SupplementalFundingAgreementCleared = request.Cleared;
            tasksData.SupplementalFundingAgreementReceived = request.Received;
            tasksData.SupplementalFundingAgreementSent = request.Sent;
            tasksData.SupplementalFundingAgreementSigned = request.Signed;
            tasksData.SupplementalFundingAgreementSignedSecretaryState = request.SignedSecretaryState;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.Now, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateSupplementalFundingAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.SupplementalFundingAgreementSaved = request.Saved;
            tasksData.SupplementalFundingAgreementCleared = request.Cleared;
            tasksData.SupplementalFundingAgreementReceived = request.Received;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.Now, cancellationToken);
        }
    }
}
