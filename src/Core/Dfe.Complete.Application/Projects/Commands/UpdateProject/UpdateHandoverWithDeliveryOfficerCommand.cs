using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateHandoverWithDeliveryOfficerCommand(
        ProjectId ProjectId,
        bool? NotApplicable,
        bool? HandoverReview,
        bool? HandoverNotes,
        bool? HandoverMeetings

    ) : IRequest<Result<bool>>;

    public class UpdateHandoverWithDeliveryOfficerCommandHandler(
        IProjectReadRepository projectReadRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository,
        ICompleteRepository<TransferTasksData> transferTaskRepository)
        : IRequestHandler<UpdateHandoverWithDeliveryOfficerCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            var project = await projectReadRepository.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
                          ?? throw new NotFoundException($"Project {request.ProjectId} not found.");

            if (project.TasksDataId == null)
            {
                return Result<bool>.Failure($"No task data associated with Project {project.Id}.");
            }
            if (project.Type == ProjectType.Conversion)
            {
                await UpdateConversionTaskDataAsync(project.TasksDataId, request, cancellationToken);
            }
            else if (project.Type == ProjectType.Transfer)
            {
                await UpdateTransferTaskDataAsync(project.TasksDataId, request, cancellationToken);
            }

            return Result<bool>.Success(true);
        }


        private async Task UpdateConversionTaskDataAsync(TaskDataId taskDataId, UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await conversionTaskRepository.FindAsync(p => p.Id == taskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");

            tasksData.HandoverMeeting = request.NotApplicable == true ? null : request.HandoverMeetings;
            tasksData.HandoverNotes = request.NotApplicable == true ? null : request.HandoverNotes;
            tasksData.HandoverReview = request.NotApplicable == true ? null : request.HandoverReview;
            tasksData.HandoverNotApplicable = request.NotApplicable;

            await conversionTaskRepository.UpdateAsync(tasksData, cancellationToken);
        }

        private async Task UpdateTransferTaskDataAsync(TaskDataId taskDataId, UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await transferTaskRepository.FindAsync(p => p.Id == taskDataId, cancellationToken) 
                ?? throw new NotFoundException($"Transfer task data {taskDataId} not found."); 

            tasksData.HandoverMeeting = request.NotApplicable == true ? null : request.HandoverMeetings;
            tasksData.HandoverNotes = request.NotApplicable == true ? null : request.HandoverNotes;
            tasksData.HandoverReview = request.NotApplicable == true ? null : request.HandoverReview;
            tasksData.HandoverNotApplicable = request.NotApplicable;

            await transferTaskRepository.UpdateAsync(tasksData, cancellationToken);
        }
    }
}
