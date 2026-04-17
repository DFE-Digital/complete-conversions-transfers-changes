using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;


namespace Dfe.Complete.Application.Projects.Commands.TaskData
{

    public record UpdateNurseryArrangementTaskCommand(
        TaskDataId TaskDataId,
        NurseryArrangementOption? NurseryArrangement
    ) : IRequest<Result<bool>>;

    internal class UpdateNurseryArrangementTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateNurseryArrangementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateNurseryArrangementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.NurseryArrangement = request.NurseryArrangement;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
