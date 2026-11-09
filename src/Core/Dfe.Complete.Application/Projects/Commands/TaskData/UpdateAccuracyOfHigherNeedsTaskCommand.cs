using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateAccuracyOfHigherNeedsTaskCommand(
        TaskDataId TaskDataId,
        bool? ConfirmNumber,
        bool? ConfirmPublishedNumber,
        bool? ConfirmPublishedIncorrectNumber,
        bool? CheckReturnedForm,
        bool? SendForm,
        bool? NotApplicable
    ) : IRequest<Result<bool>>;

    internal class UpdateAccuracyOfHigherNeedsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateAccuracyOfHigherNeedsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateAccuracyOfHigherNeedsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            if (request.NotApplicable == true)
            {
                tasksData.CheckAccuracyOfHigherNeedsNotApplicable = true;
                tasksData.CheckAccuracyOfHigherNeedsConfirmNumber = null;
                tasksData.CheckAccuracyOfHigherNeedsCheckReturnedForm = null;
                tasksData.CheckAccuracyOfHigherNeedsSendForm = null;
                tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber = null;
                tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedIncorrectNumber = null;
            }
            else if (request.ConfirmPublishedNumber == true)
            {
                tasksData.CheckAccuracyOfHigherNeedsNotApplicable = null;
                tasksData.CheckAccuracyOfHigherNeedsConfirmNumber = null;
                tasksData.CheckAccuracyOfHigherNeedsCheckReturnedForm = null;
                tasksData.CheckAccuracyOfHigherNeedsSendForm = null;
                tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber = true;
                tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedIncorrectNumber = null;
            }
            else
            {
                tasksData.CheckAccuracyOfHigherNeedsNotApplicable = null;
                tasksData.CheckAccuracyOfHigherNeedsConfirmNumber = request.ConfirmNumber;
                tasksData.CheckAccuracyOfHigherNeedsCheckReturnedForm = request.CheckReturnedForm;
                tasksData.CheckAccuracyOfHigherNeedsSendForm = request.SendForm;
                tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedNumber = null;
                tasksData.CheckAccuracyOfHigherNeedsConfirmPublishedIncorrectNumber = request.ConfirmPublishedIncorrectNumber;
            }

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
