using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdatePostDecisionActionsTaskCommand(
        TaskDataId TaskDataId,
        bool? ApplicationUploaded,
        bool? AcademyOrderUploaded,
        bool? LaProformaUploaded
    ) : IRequest<Result<bool>>;

    internal class UpdatePostDecisionActionsTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdatePostDecisionActionsTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdatePostDecisionActionsTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.PostDecisionActionsApplicationUploaded = request.ApplicationUploaded;
            tasksData.PostDecisionActionsAcademyOrderUploaded = request.AcademyOrderUploaded;
            tasksData.PostDecisionActionsLaProformaUploaded = request.LaProformaUploaded;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}