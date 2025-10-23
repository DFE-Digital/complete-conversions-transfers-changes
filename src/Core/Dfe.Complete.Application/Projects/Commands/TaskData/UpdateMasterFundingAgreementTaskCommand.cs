using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateMasterFundingAgreementTaskCommand(
        TaskDataId TaskDataId,
        [Required]
        [ProjectType]
        ProjectType? ProjectType,
        bool? NotApplicable,
        bool? Received,
        bool? Cleared,
        bool? Signed,
        bool? Saved,
        bool? Sent,
        bool? SignedSecretaryState
    ) : IRequest<Result<bool>>;

    internal class UpdateMasterFundingAgreementTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateMasterFundingAgreementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateMasterFundingAgreementTaskCommand request, CancellationToken cancellationToken)
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


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateMasterFundingAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");
             
            tasksData.MasterFundingAgreementCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.MasterFundingAgreementNotApplicable = request.NotApplicable;
            tasksData.MasterFundingAgreementReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.MasterFundingAgreementSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.MasterFundingAgreementSent = request.NotApplicable == true ? null : request.Sent;
            tasksData.MasterFundingAgreementSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.MasterFundingAgreementSignedSecretaryState = request.NotApplicable == true ? null : request.SignedSecretaryState;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateMasterFundingAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.MasterFundingAgreementCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.MasterFundingAgreementNotApplicable = request.NotApplicable;
            tasksData.MasterFundingAgreementReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.MasterFundingAgreementSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.MasterFundingAgreementSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.MasterFundingAgreementSignedSecretaryState = request.NotApplicable == true ? null : request.SignedSecretaryState;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
        }
    }
}
