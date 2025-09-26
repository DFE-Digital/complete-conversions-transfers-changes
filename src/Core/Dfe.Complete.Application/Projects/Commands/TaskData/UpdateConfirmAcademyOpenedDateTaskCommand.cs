using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmAcademyOpenedDateTaskCommand(TaskDataId TaskDataId, 
        DateOnly? AcademyOpenedDate) : IRequest<Result<bool>>;

    internal class UpdateConfirmAcademyOpenedDateTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmAcademyOpenedDateTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmAcademyOpenedDateTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.ConfirmDateAcademyOpenedDateOpened = request.AcademyOpenedDate;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
