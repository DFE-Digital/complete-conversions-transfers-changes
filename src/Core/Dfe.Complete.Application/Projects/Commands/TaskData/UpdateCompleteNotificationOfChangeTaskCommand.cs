using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateCompleteNotificationOfChangeTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? TellLocalAuthority,
        bool? CheckDocument,
        bool? SendDocument
    ) : IRequest<Result<bool>>;

    internal class UpdateCompleteNotificationOfChangeTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateCompleteNotificationOfChangeTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateCompleteNotificationOfChangeTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");
             
            tasksData.CompleteNotificationOfChangeNotApplicable = request.NotApplicable;
            tasksData.CompleteNotificationOfChangeTellLocalAuthority = request.NotApplicable == true ? null : request.TellLocalAuthority;
            tasksData.CompleteNotificationOfChangeCheckDocument = request.NotApplicable == true ? null : request.CheckDocument;
            tasksData.CompleteNotificationOfChangeSendDocument = request.NotApplicable == true ? null : request.SendDocument;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
