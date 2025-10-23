using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Utils.Exceptions;


namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateFormMTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Received,
        bool? ReceivedTitlePlans,
        bool? Cleared,
        bool? Signed,
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateFormMTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateFormMTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateFormMTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");
            
            tasksData.FormMNotApplicable = request.NotApplicable;
            tasksData.FormMReceivedFormM = request.NotApplicable == true ? null : request.Received;
            tasksData.FormMReceivedTitlePlans = request.NotApplicable == true ? null : request.ReceivedTitlePlans;
            tasksData.FormMCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.FormMSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.FormMSaved = request.NotApplicable == true ? null : request.Saved;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
