using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmSchoolBankDetailsTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? Sent,
        bool? Submitted
    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmSchoolBankDetailsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmSchoolBankDetailsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmSchoolBankDetailsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.ConfirmSchoolBankDetailsSent = request.Sent;
            tasksData.ConfirmSchoolBankDetailsSubmitted = request.Submitted;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
