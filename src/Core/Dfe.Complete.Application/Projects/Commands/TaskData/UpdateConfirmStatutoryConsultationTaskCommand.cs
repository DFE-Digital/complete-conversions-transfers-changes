using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmStatutoryConsultationTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? StatutoryConsultationComplete

    ) : IRequest<Result<bool>>;

    internal class UpdateConfirmStatutoryConsultationTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmStatutoryConsultationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmStatutoryConsultationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.StatutoryConsultationComplete = request.NotApplicable == true ? null : request.StatutoryConsultationComplete;
            tasksData.StatutoryConsultationNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.Now, cancellationToken);            return Result<bool>.Success(true);
        }
    }
}