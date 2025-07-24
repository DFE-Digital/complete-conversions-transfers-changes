using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR; 

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
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository,
        ICompleteRepository<TransferTasksData> transferTaskRepository)
        : IRequestHandler<UpdateHandoverWithDeliveryOfficerCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken)
                          ?? throw new NotFoundException($"Project {request.ProjectId} not found.");

            if (project.TasksDataId != null)
            {

                var notApplicable = request.NotApplicable.HasValue && request.NotApplicable.Value;
                if (project.Type == ProjectType.Conversion)
                {
                    await UpdateConversionTaskData(project.TasksDataId, notApplicable, request, cancellationToken);
                }
                else if (project.Type == ProjectType.Transfer)
                {
                    await UpdateTransferTaskData(project.TasksDataId, notApplicable, request, cancellationToken);
                }
            }
            return Result<bool>.Success(true);
        }


        private async Task UpdateConversionTaskData(TaskDataId taskDataId, bool notApplicable, UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await conversionTaskRepository.FindAsync(p => p.Id == taskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {taskDataId} not found.");
            tasksData.HandoverMeeting = notApplicable ? null : request.HandoverMeetings;
            tasksData.HandoverNotes = notApplicable ? null : request.HandoverNotes;
            tasksData.HandoverReview = notApplicable ? null : request.HandoverReview;

            await conversionTaskRepository.UpdateAsync(tasksData, cancellationToken);
        }

        private async Task UpdateTransferTaskData(TaskDataId taskDataId, bool notApplicable, UpdateHandoverWithDeliveryOfficerCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await transferTaskRepository.FindAsync(p => p.Id == taskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Transfer task data {taskDataId} not found.");
            
            tasksData.HandoverMeeting = notApplicable ? null : request.HandoverMeetings;
            tasksData.HandoverNotes = notApplicable ? null : request.HandoverNotes;
            tasksData.HandoverReview = notApplicable ? null : request.HandoverReview; 

            await transferTaskRepository.UpdateAsync(tasksData, cancellationToken);
        }
    }
}
