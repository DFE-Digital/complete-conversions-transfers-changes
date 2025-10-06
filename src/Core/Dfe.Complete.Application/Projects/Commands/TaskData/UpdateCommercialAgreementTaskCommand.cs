using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateCommercialAgreementTaskCommand(
        TaskDataId TaskDataId,
        ProjectType? ProjectType,
        bool? Agreed,
        bool? Signed,
        bool? QuestionsReceived,
        bool? QuestionsChecked,
        bool? Saved) : IRequest<Result<bool>>;

    internal class UpdateCommercialAgreementTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateCommercialAgreementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateCommercialAgreementTaskCommand request, CancellationToken cancellationToken)
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

        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateCommercialAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.CommercialTransferAgreementAgreed = request.Agreed;
            tasksData.CommercialTransferAgreementSaved = request.Saved;
            tasksData.CommercialTransferAgreementSigned = request.Signed;
            tasksData.CommercialTransferAgreementQuestionsReceived = request.QuestionsReceived;
            tasksData.CommercialTransferAgreementQuestionsChecked = request.QuestionsChecked;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.Now, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateCommercialAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.CommercialTransferAgreementConfirmAgreed = request.Agreed;
            tasksData.CommercialTransferAgreementSaveConfirmationEmails = request.Saved;
            tasksData.CommercialTransferAgreementConfirmSigned = request.Signed;
            tasksData.CommercialTransferAgreementQuestionsReceived = request.QuestionsReceived;
            tasksData.CommercialTransferAgreementQuestionsChecked = request.QuestionsChecked;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.Now, cancellationToken);
        }
    }
}
