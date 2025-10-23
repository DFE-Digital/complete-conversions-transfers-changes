using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmTransferHasAuthorityToProceedTaskCommand(
        TaskDataId TaskDataId, 
        bool? AnyInformationChanged,
        bool? BaselineSheetApproved,
        bool? ConfirmToProceed
    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmTransferHasAuthorityToProceedTaskCommandHandler(
        IProjectReadRepository projectReadRepository,
        IProjectWriteRepository projectWriteRepository,
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmTransferHasAuthorityToProceedTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmTransferHasAuthorityToProceedTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await projectReadRepository.Projects
                .FirstOrDefaultAsync(p => p.TasksDataId == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Project with task data id {request.TaskDataId.Value} not found.");
            var now = DateTime.Now; 
            
            project.AllConditionsMet = request.ConfirmToProceed;
            project.UpdatedAt = now;

            await projectWriteRepository.UpdateProjectAsync(project, cancellationToken);

            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.ConditionsMetCheckAnyInformationChanged = request.AnyInformationChanged;
            tasksData.ConditionsMetBaselineSheetApproved = request.BaselineSheetApproved;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, now, cancellationToken); 
            return Result<bool>.Success(true);
        }
    }
}
