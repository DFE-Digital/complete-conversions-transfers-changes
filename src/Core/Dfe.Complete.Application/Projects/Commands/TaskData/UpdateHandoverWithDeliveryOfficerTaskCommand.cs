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
    public record UpdateHandoverWithDeliveryOfficerTaskCommand(
        TaskDataId TaskDataId,
        [Required] ProjectType? ProjectType,
        bool? NotApplicable,
        bool? HandoverReview,
        bool? HandoverNotes,
        bool? HandoverConfirmSacreExemption,
        bool? HandoverMeetings

    ) : IRequest<Result<bool>>;

    internal class UpdateHandoverWithDeliveryOfficerTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateHandoverWithDeliveryOfficerTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateHandoverWithDeliveryOfficerTaskCommand request, CancellationToken cancellationToken)
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
        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateHandoverWithDeliveryOfficerTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.HandoverMeeting = request.NotApplicable == true ? null : request.HandoverMeetings;
            tasksData.HandoverNotes = request.NotApplicable == true ? null : request.HandoverNotes;
            tasksData.HandoverReview = request.NotApplicable == true ? null : request.HandoverReview;
            tasksData.HandoverConfirmSacreExemption = request.NotApplicable == true ? null : request.HandoverConfirmSacreExemption;
            tasksData.HandoverNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.Now, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateHandoverWithDeliveryOfficerTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");

            tasksData.HandoverMeeting = request.NotApplicable == true ? null : request.HandoverMeetings;
            tasksData.HandoverNotes = request.NotApplicable == true ? null : request.HandoverNotes;
            tasksData.HandoverReview = request.NotApplicable == true ? null : request.HandoverReview;
            tasksData.HandoverNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.Now, cancellationToken);
        }
    }
}
