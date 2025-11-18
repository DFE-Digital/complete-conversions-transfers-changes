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
    public record UpdateConfirmSchoolHasCompletedAllActionsTaskCommand(
        [Required] TaskDataId TaskDataId,
        bool? Emailed,
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmSchoolHasCompletedAllActionsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmSchoolHasCompletedAllActionsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmSchoolHasCompletedAllActionsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");
             
            tasksData.SchoolCompletedEmailed = request.Emailed;
            tasksData.SchoolCompletedSaved = request.Saved;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
