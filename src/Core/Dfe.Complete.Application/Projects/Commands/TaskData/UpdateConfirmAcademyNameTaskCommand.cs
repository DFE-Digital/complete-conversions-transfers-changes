using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmAcademyNameTaskCommand(TaskDataId TaskDataId, 
        string? AcademyName) : IRequest<Result<bool>>;

    internal class UpdateConfirmAcademyNameTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmAcademyNameTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmAcademyNameTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.AcademyDetailsName = request.AcademyName??string.Empty;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
