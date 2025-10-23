using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateRedactAndSendDocumentsTaskCommand(
        TaskDataId TaskDataId,
        [Required] ProjectType? ProjectType,
        bool? Redact,
        bool? Saved,
        bool? SendToEsfa,
        bool? Send,
        bool? SendToSolicitors) : IRequest<Result<bool>>;
    internal class UpdateRedactAndSendDocumentsTaskCommandHandler(
       ITaskDataReadRepository taskDataReadRepository,
       ITaskDataWriteRepository taskDataWriteRepository)
       : IRequestHandler<UpdateRedactAndSendDocumentsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateRedactAndSendDocumentsTaskCommand request, CancellationToken cancellationToken)
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
        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateRedactAndSendDocumentsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.RedactAndSendRedact = request.Redact;
            tasksData.RedactAndSendSaveRedaction = request.Saved;
            tasksData.RedactAndSendSendRedaction = request.Send;
            tasksData.RedactAndSendSendSolicitors = request.SendToSolicitors;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.Now, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateRedactAndSendDocumentsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.RedactAndSendDocumentsRedact = request.Redact;
            tasksData.RedactAndSendDocumentsSaved = request.Saved;
            tasksData.RedactAndSendDocumentsSendToFundingTeam = request.Send;
            tasksData.RedactAndSendDocumentsSendToSolicitors = request.SendToSolicitors;
            tasksData.RedactAndSendDocumentsSendToEsfa = request.SendToEsfa;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.Now, cancellationToken);
        }
    }
}
